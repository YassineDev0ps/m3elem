"use client"

import type React from "react"

import { useState, useEffect } from "react"
import { useRouter, useSearchParams } from "next/navigation"
import ProtectedRoute from "@/components/ProtectedRoute"
import SidebarLayout from "@/components/layouts/SidebarLayout"
import { useAuth } from "@/contexts/AuthContext"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Textarea } from "@/components/ui/textarea"
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select"
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from "@/components/ui/card"
import { Loader2, MapPin } from "lucide-react"
import { useToast } from "@/components/ui/use-toast"
import { generateId, saveRequest } from "@/lib/mock-service"

export default function CreateRequestPage() {
  const searchParams = useSearchParams()
  const serviceType = searchParams.get("service")
  const router = useRouter()
  const { user } = useAuth()
  const { toast } = useToast()

  const [formData, setFormData] = useState({
    title: "",
    description: "",
    serviceType: serviceType || "",
    preferredTime: "",
    latitude: "",
    longitude: "",
    address: "",
  })
  const [isLoading, setIsLoading] = useState(false)
  const [isGettingLocation, setIsGettingLocation] = useState(false)
  const [errors, setErrors] = useState<Record<string, string>>({})

  useEffect(() => {
    if (serviceType) {
      setFormData((prev) => ({ ...prev, serviceType }))
    }
  }, [serviceType])

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target
    setFormData((prev) => ({ ...prev, [name]: value }))

    // Clear error when user types
    if (errors[name]) {
      setErrors((prev) => {
        const newErrors = { ...prev }
        delete newErrors[name]
        return newErrors
      })
    }
  }

  const handleSelectChange = (name: string, value: string) => {
    setFormData((prev) => ({ ...prev, [name]: value }))

    // Clear error when user selects
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
          setFormData((prev) => ({
            ...prev,
            latitude: position.coords.latitude.toString(),
            longitude: position.coords.longitude.toString(),
          }))
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
    const newErrors: Record<string, string> = {}

    if (!formData.title.trim()) {
      newErrors.title = "Title is required"
    }

    if (!formData.description.trim()) {
      newErrors.description = "Description is required"
    }

    if (!formData.serviceType) {
      newErrors.serviceType = "Service type is required"
    }

    if (!formData.preferredTime) {
      newErrors.preferredTime = "Preferred time is required"
    }

    if (!formData.latitude || !formData.longitude) {
      newErrors.location = "Location is required"
    }

    if (!formData.address.trim()) {
      newErrors.address = "Address is required"
    }

    setErrors(newErrors)
    return Object.keys(newErrors).length === 0
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()

    if (!validateForm()) {
      return
    }

    setIsLoading(true)

    try {
      // Try to call the API first
      try {
        const response = await fetch("/api/seeker/requests", {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${user?.token}`,
          },
          body: JSON.stringify({
            title: formData.title,
            description: formData.description,
            serviceType: formData.serviceType,
            preferredTime: formData.preferredTime,
            latitude: Number.parseFloat(formData.latitude),
            longitude: Number.parseFloat(formData.longitude),
            address: formData.address,
          }),
        })

        // Check if the response is JSON
        const contentType = response.headers.get("content-type")
        if (contentType && contentType.includes("application/json")) {
          if (!response.ok) {
            const errorData = await response.json()
            throw new Error(errorData.message || "Failed to create request")
          }

          const data = await response.json()

          toast({
            title: "Request created successfully",
            description: "You can now find providers for your request.",
          })

          // Redirect to my requests page instead of select providers
          router.push("/my-requests")
          return
        }
        // If we get here, the API isn't available or isn't returning JSON
        throw new Error("API not implemented")
      } catch (error) {
        console.log("API not available, using localStorage fallback")

        // Generate a mock request with an ID
        const mockRequestId = generateId("request")
        const mockRequest = {
          id: mockRequestId,
          title: formData.title,
          description: formData.description,
          serviceType: formData.serviceType,
          preferredTime: formData.preferredTime,
          latitude: Number.parseFloat(formData.latitude),
          longitude: Number.parseFloat(formData.longitude),
          address: formData.address,
          status: "Pending", // Make sure this matches exactly with the tab value
          createdAt: new Date().toISOString(),
          seekerId: user?.id,
          providerId: null,
        }

        // Save the request
        saveRequest(mockRequest)

        console.log("Saved request to localStorage:", mockRequest)

        toast({
          title: "Request created successfully (Mock)",
          description: "You can now find providers for your request.",
        })

        // Redirect to my requests page instead of select providers
        router.push("/my-requests")
      }
    } catch (error) {
      console.error("Error creating request:", error)
      toast({
        variant: "destructive",
        title: "Failed to create request",
        description: error instanceof Error ? error.message : "An error occurred",
      })
    } finally {
      setIsLoading(false)
    }
  }

  const serviceOptions = [
    { id: "plumbing", label: "Plumbing" },
    { id: "electrical", label: "Electrical" },
    { id: "carpentry", label: "Carpentry" },
    { id: "painting", label: "Painting" },
    { id: "cleaning", label: "Cleaning" },
    { id: "moving", label: "Moving" },
    { id: "gardening", label: "Gardening" },
    { id: "appliance_repair", label: "Appliance Repair" },
  ]

  const timeOptions = [
    { id: "morning", label: "Morning (8AM - 12PM)" },
    { id: "afternoon", label: "Afternoon (12PM - 5PM)" },
    { id: "evening", label: "Evening (5PM - 9PM)" },
    { id: "flexible", label: "Flexible (Any time)" },
  ]

  return (
    <ProtectedRoute allowedRoles={["Seeker"]}>
      <SidebarLayout>
        <div className="space-y-6">
          <div>
            <h1 className="text-3xl font-bold">Create Service Request</h1>
            <p className="text-gray-500 mt-2">Provide details about the service you need</p>
          </div>

          <Card>
            <CardHeader>
              <CardTitle>Service Request Details</CardTitle>
              <CardDescription>Fill out the form below to create your service request</CardDescription>
            </CardHeader>
            <CardContent>
              <form onSubmit={handleSubmit} className="space-y-6">
                <div className="space-y-4">
                  <div className="space-y-2">
                    <Label htmlFor="title">Request Title</Label>
                    <Input
                      id="title"
                      name="title"
                      placeholder="e.g., Fix leaking bathroom sink"
                      value={formData.title}
                      onChange={handleChange}
                      className={errors.title ? "border-red-500" : ""}
                    />
                    {errors.title && <p className="text-red-500 text-xs mt-1">{errors.title}</p>}
                  </div>

                  <div className="space-y-2">
                    <Label htmlFor="description">Description</Label>
                    <Textarea
                      id="description"
                      name="description"
                      placeholder="Provide details about the service you need..."
                      value={formData.description}
                      onChange={handleChange}
                      className={errors.description ? "border-red-500" : ""}
                      rows={4}
                    />
                    {errors.description && <p className="text-red-500 text-xs mt-1">{errors.description}</p>}
                  </div>

                  <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <div className="space-y-2">
                      <Label htmlFor="serviceType">Service Type</Label>
                      <Select
                        value={formData.serviceType}
                        onValueChange={(value) => handleSelectChange("serviceType", value)}
                      >
                        <SelectTrigger id="serviceType" className={errors.serviceType ? "border-red-500" : ""}>
                          <SelectValue placeholder="Select a service type" />
                        </SelectTrigger>
                        <SelectContent>
                          {serviceOptions.map((option) => (
                            <SelectItem key={option.id} value={option.id}>
                              {option.label}
                            </SelectItem>
                          ))}
                        </SelectContent>
                      </Select>
                      {errors.serviceType && <p className="text-red-500 text-xs mt-1">{errors.serviceType}</p>}
                    </div>

                    <div className="space-y-2">
                      <Label htmlFor="preferredTime">Preferred Time</Label>
                      <Select
                        value={formData.preferredTime}
                        onValueChange={(value) => handleSelectChange("preferredTime", value)}
                      >
                        <SelectTrigger id="preferredTime" className={errors.preferredTime ? "border-red-500" : ""}>
                          <SelectValue placeholder="Select preferred time" />
                        </SelectTrigger>
                        <SelectContent>
                          {timeOptions.map((option) => (
                            <SelectItem key={option.id} value={option.id}>
                              {option.label}
                            </SelectItem>
                          ))}
                        </SelectContent>
                      </Select>
                      {errors.preferredTime && <p className="text-red-500 text-xs mt-1">{errors.preferredTime}</p>}
                    </div>
                  </div>

                  <div className="space-y-2">
                    <Label htmlFor="address">Address</Label>
                    <Input
                      id="address"
                      name="address"
                      placeholder="e.g., 123 Main St, Casablanca"
                      value={formData.address}
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
                            Get Current Location
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
                          placeholder="31.7917"
                          value={formData.latitude}
                          onChange={handleChange}
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
                          placeholder="-7.0926"
                          value={formData.longitude}
                          onChange={handleChange}
                          className={errors.location ? "border-red-500" : ""}
                        />
                      </div>
                    </div>

                    {errors.location && <p className="text-red-500 text-xs mt-1">{errors.location}</p>}
                  </div>
                </div>

                <CardFooter className="px-0 pt-4 flex justify-between">
                  <Button type="button" variant="outline" onClick={() => router.back()}>
                    Back
                  </Button>
                  <Button type="submit" disabled={isLoading}>
                    {isLoading ? (
                      <>
                        <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                        Creating...
                      </>
                    ) : (
                      "Create Request"
                    )}
                  </Button>
                </CardFooter>
              </form>
            </CardContent>
          </Card>
        </div>
      </SidebarLayout>
    </ProtectedRoute>
  )
}
