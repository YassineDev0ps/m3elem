"use client"

import { useState, useEffect } from "react"
import { useRouter, useSearchParams } from "next/navigation"
import ProtectedRoute from "@/components/ProtectedRoute"
import SidebarLayout from "@/components/layouts/SidebarLayout"
import WorkerCard from "@/components/WorkerCard"
import { useAuth } from "@/contexts/AuthContext"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from "@/components/ui/card"
import { Loader2, Search, Filter } from "lucide-react"
import { useToast } from "@/components/ui/use-toast"
import { getRandomProviders, updateRequest, getRequestById } from "@/lib/mock-service"
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select"
import { Checkbox } from "@/components/ui/checkbox"

interface Provider {
  id: string
  fullName: string
  rating: number
  reviewCount: number
  distance: number
  skills: string[]
  availability: string[]
  experience: number
  completedJobs: number
}

export default function SearchProvidersPage() {
  const searchParams = useSearchParams()
  const requestId = searchParams.get("requestId")
  const router = useRouter()
  const { user } = useAuth()
  const { toast } = useToast()

  const [providers, setProviders] = useState<Provider[]>([])
  const [filteredProviders, setFilteredProviders] = useState<Provider[]>([])
  const [selectedProvider, setSelectedProvider] = useState<string | null>(null)
  const [isLoading, setIsLoading] = useState(true)
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [request, setRequest] = useState<any>(null)

  // Search and filter states
  const [searchTerm, setSearchTerm] = useState("")
  const [filters, setFilters] = useState({
    minRating: 0,
    maxDistance: 10,
    skills: [] as string[],
  })
  const [showFilters, setShowFilters] = useState(false)

  useEffect(() => {
    if (!requestId) {
      toast({
        variant: "destructive",
        title: "Missing request ID",
        description: "Redirecting to home page...",
      })
      router.push("/home")
      return
    }

    fetchRequestDetails()
    fetchProviders()
  }, [requestId])

  useEffect(() => {
    if (providers.length > 0) {
      applyFilters()
    }
  }, [providers, searchTerm, filters])

  const fetchRequestDetails = async () => {
    if (!requestId) return

    try {
      // Try to call the API first
      try {
        const response = await fetch(`/api/seeker/requests/${requestId}`, {
          headers: {
            Authorization: `Bearer ${user?.token}`,
          },
        })

        // Check if the response is JSON
        const contentType = response.headers.get("content-type")
        if (contentType && contentType.includes("application/json")) {
          if (!response.ok) {
            throw new Error("Failed to fetch request details")
          }

          const data = await response.json()
          setRequest(data)
          return
        }
        // If we get here, the API isn't available or isn't returning JSON
        throw new Error("API not implemented")
      } catch (error) {
        console.log("API not available, using localStorage fallback")

        // Get request from localStorage
        const req = getRequestById(requestId)
        if (req) {
          setRequest(req)
        } else {
          toast({
            variant: "destructive",
            title: "Request not found",
            description: "The requested job could not be found.",
          })
          router.push("/my-requests")
        }
      }
    } catch (error) {
      console.error("Error fetching request details:", error)
      toast({
        variant: "destructive",
        title: "Failed to fetch request details",
        description: "Please try again later.",
      })
    }
  }

  const fetchProviders = async () => {
    if (!user || !requestId) return

    setIsLoading(true)
    try {
      // Try to call the API first
      try {
        const response = await fetch(`/api/providers/search`, {
          headers: {
            Authorization: `Bearer ${user.token}`,
          },
        })

        // Check if the response is JSON
        const contentType = response.headers.get("content-type")
        if (contentType && contentType.includes("application/json")) {
          if (!response.ok) {
            throw new Error("Failed to fetch providers")
          }

          const data = await response.json()
          setProviders(data)
          setFilteredProviders(data)
          return
        }
        // If we get here, the API isn't available or isn't returning JSON
        throw new Error("API not implemented")
      } catch (error) {
        console.log("API not available, using localStorage fallback")

        // Get random providers from the mock service
        const mockProviders = getRandomProviders(6).map((provider) => ({
          id: provider.id,
          fullName: provider.fullName,
          rating: provider.rating || 4.5,
          reviewCount: provider.reviewCount || 10,
          distance: Math.random() * 3, // Random distance between 0-3km
          skills: provider.skills || [],
          availability: provider.availability || [],
          experience: provider.experience || 1,
          completedJobs: provider.completedJobs || 0,
        }))

        setProviders(mockProviders)
        setFilteredProviders(mockProviders)
      }
    } catch (error) {
      console.error("Error fetching providers:", error)
      toast({
        variant: "destructive",
        title: "Failed to fetch providers",
        description: "Please try again later.",
      })
    } finally {
      setIsLoading(false)
    }
  }

  const applyFilters = () => {
    let result = [...providers]

    // Apply search term filter
    if (searchTerm) {
      const term = searchTerm.toLowerCase()
      result = result.filter((provider) => provider.fullName.toLowerCase().includes(term))
    }

    // Apply rating filter
    if (filters.minRating > 0) {
      result = result.filter((provider) => provider.rating >= filters.minRating)
    }

    // Apply distance filter
    if (filters.maxDistance < 10) {
      result = result.filter((provider) => provider.distance <= filters.maxDistance)
    }

    // Apply skills filter
    if (filters.skills.length > 0) {
      result = result.filter((provider) => filters.skills.some((skill) => provider.skills.includes(skill)))
    }

    setFilteredProviders(result)
  }

  const handleSelectProvider = (providerId: string) => {
    setSelectedProvider(providerId)
  }

  const handleSubmit = async () => {
    if (!selectedProvider || !requestId || !user) return

    setIsSubmitting(true)
    try {
      // Try to call the API first
      try {
        const response = await fetch(`/api/seeker/requests/${requestId}/select-provider`, {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${user.token}`,
          },
          body: JSON.stringify({
            providerId: selectedProvider,
          }),
        })

        // Check if the response is JSON
        const contentType = response.headers.get("content-type")
        if (contentType && contentType.includes("application/json")) {
          if (!response.ok) {
            throw new Error("Failed to select provider")
          }

          toast({
            title: "Provider selected successfully",
            description: "Your request has been sent to the provider.",
          })

          router.push("/my-requests")
          return
        }
        // If we get here, the API isn't available or isn't returning JSON
        throw new Error("API not implemented")
      } catch (error) {
        console.log("API not available, using localStorage fallback")

        // Update the request in localStorage
        updateRequest(requestId, {
          providerId: selectedProvider,
          status: "Pending", // Changed from "Accepted" to "Pending"
        })

        toast({
          title: "Provider selected successfully (Mock)",
          description: "Using localStorage mock implementation since the API isn't available yet.",
        })

        router.push("/my-requests")
      }
    } catch (error) {
      console.error("Error selecting provider:", error)
      toast({
        variant: "destructive",
        title: "Failed to select provider",
        description: "Please try again later.",
      })
    } finally {
      setIsSubmitting(false)
    }
  }

  const handleFilterChange = (key: string, value: any) => {
    setFilters((prev) => ({
      ...prev,
      [key]: value,
    }))
  }

  return (
    <ProtectedRoute allowedRoles={["Seeker"]}>
      <SidebarLayout>
        <div className="space-y-6">
          <div>
            <h1 className="text-3xl font-bold">Find Service Providers</h1>
            <p className="text-gray-500 mt-2">
              {request ? `Search for providers for: "${request.title}"` : "Search for service providers"}
            </p>
          </div>

          <Card>
            <CardHeader>
              <CardTitle>Search Providers</CardTitle>
              <CardDescription>Find the right professional for your job</CardDescription>
            </CardHeader>
            <CardContent>
              <div className="space-y-4">
                <div className="flex gap-2">
                  <div className="flex-1">
                    <div className="relative">
                      <Search className="absolute left-2.5 top-2.5 h-4 w-4 text-gray-500" />
                      <Input
                        type="search"
                        placeholder="Search by name..."
                        className="pl-9"
                        value={searchTerm}
                        onChange={(e) => setSearchTerm(e.target.value)}
                      />
                    </div>
                  </div>
                  <Button
                    variant="outline"
                    onClick={() => setShowFilters(!showFilters)}
                    className="flex items-center gap-2"
                  >
                    <Filter className="h-4 w-4" />
                    Filters
                  </Button>
                </div>

                {showFilters && (
                  <div className="bg-gray-50 p-4 rounded-md space-y-4">
                    <div>
                      <Label>Minimum Rating</Label>
                      <Select
                        value={filters.minRating.toString()}
                        onValueChange={(value) => handleFilterChange("minRating", Number(value))}
                      >
                        <SelectTrigger>
                          <SelectValue placeholder="Any rating" />
                        </SelectTrigger>
                        <SelectContent>
                          <SelectItem value="0">Any rating</SelectItem>
                          <SelectItem value="3">3+ stars</SelectItem>
                          <SelectItem value="4">4+ stars</SelectItem>
                          <SelectItem value="4.5">4.5+ stars</SelectItem>
                        </SelectContent>
                      </Select>
                    </div>

                    <div>
                      <Label>Maximum Distance</Label>
                      <Select
                        value={filters.maxDistance.toString()}
                        onValueChange={(value) => handleFilterChange("maxDistance", Number(value))}
                      >
                        <SelectTrigger>
                          <SelectValue placeholder="Any distance" />
                        </SelectTrigger>
                        <SelectContent>
                          <SelectItem value="10">Any distance</SelectItem>
                          <SelectItem value="1">Within 1 km</SelectItem>
                          <SelectItem value="2">Within 2 km</SelectItem>
                          <SelectItem value="5">Within 5 km</SelectItem>
                        </SelectContent>
                      </Select>
                    </div>

                    <div>
                      <Label className="mb-2 block">Skills</Label>
                      <div className="grid grid-cols-2 gap-2">
                        {["plumbing", "electrical", "carpentry", "painting", "cleaning", "moving"].map((skill) => (
                          <div key={skill} className="flex items-center space-x-2">
                            <Checkbox
                              id={`skill-${skill}`}
                              checked={filters.skills.includes(skill)}
                              onCheckedChange={(checked) => {
                                if (checked) {
                                  handleFilterChange("skills", [...filters.skills, skill])
                                } else {
                                  handleFilterChange(
                                    "skills",
                                    filters.skills.filter((s) => s !== skill),
                                  )
                                }
                              }}
                            />
                            <Label htmlFor={`skill-${skill}`} className="text-sm font-normal capitalize">
                              {skill}
                            </Label>
                          </div>
                        ))}
                      </div>
                    </div>
                  </div>
                )}

                {isLoading ? (
                  <div className="flex justify-center items-center py-12">
                    <Loader2 className="h-8 w-8 animate-spin text-primary" />
                    <span className="ml-2 text-lg">Finding providers...</span>
                  </div>
                ) : filteredProviders.length === 0 ? (
                  <div className="text-center py-12">
                    <p className="text-lg text-gray-500">No providers match your search criteria.</p>
                    <Button
                      className="mt-4"
                      onClick={() => {
                        setSearchTerm("")
                        setFilters({
                          minRating: 0,
                          maxDistance: 10,
                          skills: [],
                        })
                      }}
                    >
                      Clear Filters
                    </Button>
                  </div>
                ) : (
                  <div className="space-y-6">
                    {filteredProviders.map((provider) => (
                      <WorkerCard
                        key={provider.id}
                        worker={provider}
                        onSelect={handleSelectProvider}
                        selected={selectedProvider === provider.id}
                      />
                    ))}
                  </div>
                )}
              </div>
            </CardContent>
            <CardFooter className="flex justify-between">
              <Button variant="outline" onClick={() => router.back()} disabled={isSubmitting}>
                Back
              </Button>
              <Button onClick={handleSubmit} disabled={!selectedProvider || isSubmitting}>
                {isSubmitting ? (
                  <>
                    <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                    Sending Request...
                  </>
                ) : (
                  "Send Request to Provider"
                )}
              </Button>
            </CardFooter>
          </Card>
        </div>
      </SidebarLayout>
    </ProtectedRoute>
  )
}
