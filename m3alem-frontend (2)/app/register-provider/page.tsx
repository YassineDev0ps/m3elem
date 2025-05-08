"use client"

import type React from "react"

import { useState } from "react"
import Link from "next/link"
import { useAuth } from "@/contexts/AuthContext"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Textarea } from "@/components/ui/textarea"
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from "@/components/ui/card"
import { Checkbox } from "@/components/ui/checkbox"
import { Loader2, MapPin } from "lucide-react"

export default function RegisterProviderPage() {
  const [formData, setFormData] = useState({
    fullName: "",
    email: "",
    phone: "",
    password: "",
    confirmPassword: "",
    bio: "",
    experience: "",
    latitude: "",
    longitude: "",
    skills: [] as string[],
    availability: [] as string[],
  })
  const [isLoading, setIsLoading] = useState(false)
  const [isGettingLocation, setIsGettingLocation] = useState(false)
  const [errors, setErrors] = useState<Record<string, string>>({})
  const { registerProvider } = useAuth()

  const skillOptions = [
    { id: "plumbing", label: "Plumbing" },
    { id: "electrical", label: "Electrical" },
    { id: "carpentry", label: "Carpentry" },
    { id: "painting", label: "Painting" },
    { id: "cleaning", label: "Cleaning" },
    { id: "moving", label: "Moving" },
    { id: "gardening", label: "Gardening" },
    { id: "appliance_repair", label: "Appliance Repair" },
  ]

  const availabilityOptions = [
    { id: "weekday_morning", label: "Weekday Mornings" },
    { id: "weekday_afternoon", label: "Weekday Afternoons" },
    { id: "weekday_evening", label: "Weekday Evenings" },
    { id: "weekend_morning", label: "Weekend Mornings" },
    { id: "weekend_afternoon", label: "Weekend Afternoons" },
    { id: "weekend_evening", label: "Weekend Evenings" },
  ]

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

  const handleSkillChange = (skillId: string, checked: boolean) => {
    setFormData((prev) => {
      if (checked) {
        return { ...prev, skills: [...prev.skills, skillId] }
      } else {
        return { ...prev, skills: prev.skills.filter((id) => id !== skillId) }
      }
    })

    // Clear error when user selects
    if (errors.skills) {
      setErrors((prev) => {
        const newErrors = { ...prev }
        delete newErrors.skills
        return newErrors
      })
    }
  }

  const handleAvailabilityChange = (availabilityId: string, checked: boolean) => {
    setFormData((prev) => {
      if (checked) {
        return { ...prev, availability: [...prev.availability, availabilityId] }
      } else {
        return { ...prev, availability: prev.availability.filter((id) => id !== availabilityId) }
      }
    })

    // Clear error when user selects
    if (errors.availability) {
      setErrors((prev) => {
        const newErrors = { ...prev }
        delete newErrors.availability
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

    if (!formData.fullName.trim()) {
      newErrors.fullName = "Full name is required"
    }

    if (!formData.email.trim()) {
      newErrors.email = "Email is required"
    } else if (!/\S+@\S+\.\S+/.test(formData.email)) {
      newErrors.email = "Email is invalid"
    }

    if (!formData.phone.trim()) {
      newErrors.phone = "Phone number is required"
    } else if (!/^\+?\d{8,15}$/.test(formData.phone.replace(/\s/g, ""))) {
      newErrors.phone = "Phone number is invalid"
    }

    if (!formData.password) {
      newErrors.password = "Password is required"
    } else if (formData.password.length < 6) {
      newErrors.password = "Password must be at least 6 characters"
    }

    if (formData.password !== formData.confirmPassword) {
      newErrors.confirmPassword = "Passwords do not match"
    }

    if (!formData.bio.trim()) {
      newErrors.bio = "Bio is required"
    }

    if (!formData.experience.trim()) {
      newErrors.experience = "Experience is required"
    }

    if (formData.skills.length === 0) {
      newErrors.skills = "Please select at least one skill"
    }

    if (formData.availability.length === 0) {
      newErrors.availability = "Please select at least one availability option"
    }

    if (!formData.latitude || !formData.longitude) {
      newErrors.location = "Location is required"
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
      await registerProvider({
        fullName: formData.fullName,
        email: formData.email,
        phone: formData.phone,
        password: formData.password,
        bio: formData.bio,
        experience: formData.experience,
        latitude: Number.parseFloat(formData.latitude),
        longitude: Number.parseFloat(formData.longitude),
        skills: formData.skills,
        availability: formData.availability,
      })
      // Redirect is handled in the AuthContext after successful registration
    } catch (error) {
      console.error("Registration error:", error)
      // Error is already handled in the AuthContext with toast notifications
    } finally {
      setIsLoading(false)
    }
  }

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50 py-12 px-4 sm:px-6 lg:px-8">
      <div className="max-w-md w-full space-y-8">
        <div className="text-center">
          <Link href="/" className="inline-flex items-center space-x-2">
            <div className="bg-primary rounded-md p-1">
              <span className="text-white font-bold text-xl">M3</span>
            </div>
            <span className="font-bold text-xl">M3alem.ma</span>
          </Link>
          <h2 className="mt-6 text-3xl font-extrabold text-gray-900">Create a Provider Account</h2>
          <p className="mt-2 text-sm text-gray-600">
            Or{" "}
            <Link href="/login" className="font-medium text-primary hover:text-primary/80">
              sign in to your existing account
            </Link>
          </p>
        </div>

        <Card>
          <CardHeader>
            <CardTitle>Register as Service Provider</CardTitle>
            <CardDescription>Create an account to offer your services to customers</CardDescription>
          </CardHeader>
          <CardContent>
            <form onSubmit={handleSubmit} className="space-y-4">
              <div className="space-y-2">
                <Label htmlFor="fullName">Full Name</Label>
                <Input
                  id="fullName"
                  name="fullName"
                  placeholder="John Doe"
                  value={formData.fullName}
                  onChange={handleChange}
                  className={errors.fullName ? "border-red-500" : ""}
                />
                {errors.fullName && <p className="text-red-500 text-xs mt-1">{errors.fullName}</p>}
              </div>

              <div className="space-y-2">
                <Label htmlFor="email">Email</Label>
                <Input
                  id="email"
                  name="email"
                  type="email"
                  placeholder="name@example.com"
                  value={formData.email}
                  onChange={handleChange}
                  className={errors.email ? "border-red-500" : ""}
                />
                {errors.email && <p className="text-red-500 text-xs mt-1">{errors.email}</p>}
              </div>

              <div className="space-y-2">
                <Label htmlFor="phone">Phone Number</Label>
                <Input
                  id="phone"
                  name="phone"
                  placeholder="+212 6XX-XXXXXX"
                  value={formData.phone}
                  onChange={handleChange}
                  className={errors.phone ? "border-red-500" : ""}
                />
                {errors.phone && <p className="text-red-500 text-xs mt-1">{errors.phone}</p>}
              </div>

              <div className="space-y-2">
                <Label htmlFor="password">Password</Label>
                <Input
                  id="password"
                  name="password"
                  type="password"
                  placeholder="••••••••"
                  value={formData.password}
                  onChange={handleChange}
                  className={errors.password ? "border-red-500" : ""}
                />
                {errors.password && <p className="text-red-500 text-xs mt-1">{errors.password}</p>}
              </div>

              <div className="space-y-2">
                <Label htmlFor="confirmPassword">Confirm Password</Label>
                <Input
                  id="confirmPassword"
                  name="confirmPassword"
                  type="password"
                  placeholder="••••••••"
                  value={formData.confirmPassword}
                  onChange={handleChange}
                  className={errors.confirmPassword ? "border-red-500" : ""}
                />
                {errors.confirmPassword && <p className="text-red-500 text-xs mt-1">{errors.confirmPassword}</p>}
              </div>

              <div className="space-y-2">
                <Label htmlFor="bio">Bio</Label>
                <Textarea
                  id="bio"
                  name="bio"
                  placeholder="Tell customers about yourself and your services..."
                  value={formData.bio}
                  onChange={handleChange}
                  className={errors.bio ? "border-red-500" : ""}
                  rows={3}
                />
                {errors.bio && <p className="text-red-500 text-xs mt-1">{errors.bio}</p>}
              </div>

              <div className="space-y-2">
                <Label htmlFor="experience">Years of Experience</Label>
                <Input
                  id="experience"
                  name="experience"
                  type="number"
                  placeholder="5"
                  value={formData.experience}
                  onChange={handleChange}
                  className={errors.experience ? "border-red-500" : ""}
                  min="0"
                />
                {errors.experience && <p className="text-red-500 text-xs mt-1">{errors.experience}</p>}
              </div>

              <div className="space-y-2">
                <Label>Skills</Label>
                <div className="grid grid-cols-2 gap-2 mt-1">
                  {skillOptions.map((skill) => (
                    <div key={skill.id} className="flex items-center space-x-2">
                      <Checkbox
                        id={`skill-${skill.id}`}
                        checked={formData.skills.includes(skill.id)}
                        onCheckedChange={(checked) => handleSkillChange(skill.id, checked as boolean)}
                      />
                      <Label htmlFor={`skill-${skill.id}`} className="text-sm font-normal">
                        {skill.label}
                      </Label>
                    </div>
                  ))}
                </div>
                {errors.skills && <p className="text-red-500 text-xs mt-1">{errors.skills}</p>}
              </div>

              <div className="space-y-2">
                <Label>Availability</Label>
                <div className="grid grid-cols-2 gap-2 mt-1">
                  {availabilityOptions.map((availability) => (
                    <div key={availability.id} className="flex items-center space-x-2">
                      <Checkbox
                        id={`availability-${availability.id}`}
                        checked={formData.availability.includes(availability.id)}
                        onCheckedChange={(checked) => handleAvailabilityChange(availability.id, checked as boolean)}
                      />
                      <Label htmlFor={`availability-${availability.id}`} className="text-sm font-normal">
                        {availability.label}
                      </Label>
                    </div>
                  ))}
                </div>
                {errors.availability && <p className="text-red-500 text-xs mt-1">{errors.availability}</p>}
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

              <Button type="submit" className="w-full" disabled={isLoading}>
                {isLoading ? (
                  <>
                    <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                    Creating account...
                  </>
                ) : (
                  "Create Provider Account"
                )}
              </Button>
            </form>
          </CardContent>
          <CardFooter className="flex justify-center">
            <p className="text-sm text-gray-600">
              Looking for services?{" "}
              <Link href="/register-seeker" className="font-medium text-primary hover:text-primary/80">
                Register as Seeker
              </Link>
            </p>
          </CardFooter>
        </Card>
      </div>
    </div>
  )
}
