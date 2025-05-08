"use client"

import type React from "react"

import { useState, useEffect } from "react"
import ProtectedRoute from "@/components/ProtectedRoute"
import SidebarLayout from "@/components/layouts/SidebarLayout"
import { useAuth } from "@/contexts/AuthContext"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from "@/components/ui/card"
import { Loader2, MapPin } from "lucide-react"
import { useToast } from "@/components/ui/use-toast"

interface ProfileData {
  fullName: string
  email: string
  phone: string
  latitude: number
  longitude: number
  address: string
}

export default function SeekerProfilePage() {
  const [profileData, setProfileData] = useState<ProfileData | null>(null)
  const [isLoading, setIsLoading] = useState(true)
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [isGettingLocation, setIsGettingLocation] = useState(false)
  const [errors, setErrors] = useState<Record<string, string>>({})
  const { user } = useAuth()
  const { toast } = useToast()

  useEffect(() => {
    fetchProfileData()
  }, [])

  // Update the fetchProfileData function to properly handle non-JSON responses
  const fetchProfileData = async () => {
    if (!user) return

    setIsLoading(true)
    try {
      // Try to call the API first
      try {
        const response = await fetch("/api/seeker/profile", {
          headers: {
            Authorization: `Bearer ${user.token}`,
          },
        })

        // Check if the response is JSON
        const contentType = response.headers.get("content-type")
        if (contentType && contentType.includes("application/json")) {
          if (!response.ok) {
            throw new Error("Failed to fetch profile data")
          }

          const data = await response.json()
          setProfileData(data)
          return
        }
        // If we get here, the API isn't available or isn't returning JSON
        throw new Error("API not implemented")
      } catch (error) {
        console.log("API not available, using localStorage fallback")

        // Fall back to localStorage implementation
        // Get user data from localStorage
        const users = localStorage.getItem("m3alem_users")
        if (users) {
          const parsedUsers = JSON.parse(users)
          const currentUser = parsedUsers.find((u: any) => u.id === user.id)

          if (currentUser) {
            // Create profile data from user data
            const mockProfileData = {
              fullName: currentUser.fullName || user.fullName,
              email: currentUser.email,
              phone: currentUser.phone || "",
              latitude: currentUser.latitude || 0,
              longitude: currentUser.longitude || 0,
              address: currentUser.address || "",
            }

            setProfileData(mockProfileData)
            return
          }
        }

        // If no user data found, create default profile data
        const defaultProfileData = {
          fullName: user.fullName,
          email: user.email || "",
          phone: "",
          latitude: 0,
          longitude: 0,
          address: "",
        }

        setProfileData(defaultProfileData)
      }
    } catch (error) {
      console.error("Error fetching profile data:", error)
      toast({
        variant: "destructive",
        title: "Failed to fetch profile data",
        description: "Please try again later.",
      })
    } finally {
      setIsLoading(false)
    }
  }

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (!profileData) return

    const { name, value } = e.target
    setProfileData({ ...profileData, [name]: value })

    // Clear error when user types
    if (errors[name]) {
      setErrors((prev) => {
        const newErrors = { ...prev }
        delete newErrors[name]
        return newErrors
      })
    }
  }

  const getLocation = () => {
    setIsGettingLocation(true)
    if (navigator.geolocation) {
      navigator.geolocation.getCurrentPosition(
        (position) => {
          if (profileData) {
            setProfileData({
              ...profileData,
              latitude: position.coords.latitude,
              longitude: position.coords.longitude,
            })
          }
          setIsGettingLocation(false)
        },
        (error) => {
          console.error("Error getting location:", error)
          setErrors((prev) => ({
            ...prev,
            location: "Failed to get your location. Please try again or enter manually.",
          }))
          setIsGettingLocation(false)
        },
      )
    } else {
      setErrors((prev) => ({
        ...prev,
        location: "Geolocation is not supported by your browser.",
      }))
      setIsGettingLocation(false)
    }
  }

  const validateForm = () => {
    if (!profileData) return false

    const newErrors: Record<string, string> = {}

    if (!profileData.fullName.trim()) {
      newErrors.fullName = "Full name is required"
    }

    if (!profileData.phone.trim()) {
      newErrors.phone = "Phone number is required"
    } else if (!/^\+?\d{8,15}$/.test(profileData.phone.replace(/\s/g, ""))) {
      newErrors.phone = "Phone number is invalid"
    }

    if (!profileData.latitude || !profileData.longitude) {
      newErrors.location = "Location is required"
    }

    if (!profileData.address.trim()) {
      newErrors.address = "Address is required"
    }

    setErrors(newErrors)
    return Object.keys(newErrors).length === 0
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()

    if (!validateForm() || !profileData || !user) {
      return
    }

    setIsSubmitting(true)

    try {
      const response = await fetch("/api/seeker/profile", {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${user.token}`,
        },
        body: JSON.stringify(profileData),
      })

      if (!response.ok) {
        const errorData = await response.json()
        throw new Error(errorData.message || "Failed to update profile")
      }

      toast({
        title: "Profile updated",
        description: "Your profile has been updated successfully.",
      })
    } catch (error) {
      console.error("Error updating profile:", error)
      toast({
        variant: "destructive",
        title: "Failed to update profile",
        description: error instanceof Error ? error.message : "An error occurred",
      })
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <ProtectedRoute allowedRoles={["Seeker"]}>
      <SidebarLayout>
        <div className="space-y-6">
          <div>
            <h1 className="text-3xl font-bold">My Profile</h1>
            <p className="text-gray-500 mt-2">Manage your personal information</p>
          </div>

          {isLoading ? (
            <div className="flex justify-center items-center py-12">
              <Loader2 className="h-8 w-8 animate-spin text-primary" />
              <span className="ml-2">Loading profile data...</span>
            </div>
          ) : !profileData ? (
            <div className="text-center py-12">
              <p className="text-lg text-gray-500">Failed to load profile data. Please try again later.</p>
              <Button className="mt-4" onClick={fetchProfileData}>
                Retry
              </Button>
            </div>
          ) : (
            <Card>
              <CardHeader>
                <CardTitle>Edit Profile</CardTitle>
                <CardDescription>Update your profile information</CardDescription>
              </CardHeader>
              <CardContent>
                <form onSubmit={handleSubmit} className="space-y-6">
                  <div className="space-y-4">
                    <div className="space-y-2">
                      <Label htmlFor="fullName">Full Name</Label>
                      <Input
                        id="fullName"
                        name="fullName"
                        value={profileData.fullName}
                        onChange={handleChange}
                        className={errors.fullName ? "border-red-500" : ""}
                      />
                      {errors.fullName && <p className="text-red-500 text-xs mt-1">{errors.fullName}</p>}
                    </div>

                    <div className="space-y-2">
                      <Label htmlFor="email">Email</Label>
                      <Input id="email" name="email" value={profileData.email} disabled className="bg-gray-100" />
                      <p className="text-xs text-gray-500">Email cannot be changed</p>
                    </div>

                    <div className="space-y-2">
                      <Label htmlFor="phone">Phone Number</Label>
                      <Input
                        id="phone"
                        name="phone"
                        value={profileData.phone}
                        onChange={handleChange}
                        className={errors.phone ? "border-red-500" : ""}
                      />
                      {errors.phone && <p className="text-red-500 text-xs mt-1">{errors.phone}</p>}
                    </div>

                    <div className="space-y-2">
                      <Label htmlFor="address">Address</Label>
                      <Input
                        id="address"
                        name="address"
                        value={profileData.address}
                        onChange={handleChange}
                        className={errors.address ? "border-red-500" : ""}
                      />
                      {errors.address && <p className="text-red-500 text-xs mt-1">{errors.address}</p>}
                    </div>

                    <div className="space-y-2">
                      <Label>Location</Label>
                      <div className="flex space-x-2">
                        <Button
                          type="button"
                          variant="outline"
                          className="flex-1"
                          onClick={getLocation}
                          disabled={isGettingLocation}
                        >
                          {isGettingLocation ? (
                            <>
                              <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                              Getting location...
                            </>
                          ) : (
                            <>
                              <MapPin className="mr-2 h-4 w-4" />
                              Update Current Location
                            </>
                          )}
                        </Button>
                      </div>

                      <div className="grid grid-cols-2 gap-2 mt-2">
                        <div>
                          <Label htmlFor="latitude" className="text-xs">
                            Latitude
                          </Label>
                          <Input
                            id="latitude"
                            name="latitude"
                            value={profileData.latitude.toString()}
                            onChange={(e) =>
                              setProfileData({
                                ...profileData,
                                latitude: Number.parseFloat(e.target.value) || 0,
                              })
                            }
                            className={errors.location ? "border-red-500" : ""}
                          />
                        </div>
                        <div>
                          <Label htmlFor="longitude" className="text-xs">
                            Longitude
                          </Label>
                          <Input
                            id="longitude"
                            name="longitude"
                            value={profileData.longitude.toString()}
                            onChange={(e) =>
                              setProfileData({
                                ...profileData,
                                longitude: Number.parseFloat(e.target.value) || 0,
                              })
                            }
                            className={errors.location ? "border-red-500" : ""}
                          />
                        </div>
                      </div>

                      {errors.location && <p className="text-red-500 text-xs mt-1">{errors.location}</p>}
                    </div>
                  </div>

                  <CardFooter className="px-0 pt-4">
                    <Button type="submit" className="ml-auto" disabled={isSubmitting}>
                      {isSubmitting ? (
                        <>
                          <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                          Saving...
                        </>
                      ) : (
                        "Save Changes"
                      )}
                    </Button>
                  </CardFooter>
                </form>
              </CardContent>
            </Card>
          )}
        </div>
      </SidebarLayout>
    </ProtectedRoute>
  )
}
