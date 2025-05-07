"use client"

import { useState, useEffect } from "react"
import { useRouter, useSearchParams } from "next/navigation"
import ProtectedRoute from "@/components/ProtectedRoute"
import SidebarLayout from "@/components/layouts/SidebarLayout"
import WorkerCard from "@/components/WorkerCard"
import { useAuth } from "@/contexts/AuthContext"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from "@/components/ui/card"
import { Loader2 } from "lucide-react"
import { useToast } from "@/components/ui/use-toast"
import { getRandomProviders, updateRequest } from "@/lib/mock-service"

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

export default function SelectProvidersPage() {
  const searchParams = useSearchParams()
  const requestId = searchParams.get("requestId")
  const router = useRouter()
  const { user } = useAuth()
  const { toast } = useToast()

  const [providers, setProviders] = useState<Provider[]>([])
  const [selectedProvider, setSelectedProvider] = useState<string | null>(null)
  const [isLoading, setIsLoading] = useState(true)
  const [isSubmitting, setIsSubmitting] = useState(false)

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

    fetchProviders()
  }, [requestId])

  const fetchProviders = async () => {
    if (!user || !requestId) return

    setIsLoading(true)
    try {
      // Try to call the API first
      try {
        const response = await fetch(`/api/match?requestId=${requestId}`, {
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
          return
        }
        // If we get here, the API isn't available or isn't returning JSON
        throw new Error("API not implemented")
      } catch (error) {
        console.log("API not available, using localStorage fallback")

        // Get random providers from the mock service
        const mockProviders = getRandomProviders(3).map((provider) => ({
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
          status: "Accepted",
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

  return (
    <ProtectedRoute allowedRoles={["Seeker"]}>
      <SidebarLayout>
        <div className="space-y-6">
          <div>
            <h1 className="text-3xl font-bold">Select a Service Provider</h1>
            <p className="text-gray-500 mt-2">Choose from our recommended providers for your request</p>
          </div>

          <Card>
            <CardHeader>
              <CardTitle>Available Providers</CardTitle>
              <CardDescription>We've found {providers.length} providers that match your request</CardDescription>
            </CardHeader>
            <CardContent>
              {isLoading ? (
                <div className="flex justify-center items-center py-12">
                  <Loader2 className="h-8 w-8 animate-spin text-primary" />
                  <span className="ml-2 text-lg">Finding providers...</span>
                </div>
              ) : providers.length === 0 ? (
                <div className="text-center py-12">
                  <p className="text-lg text-gray-500">No providers available for your request at this time.</p>
                  <Button className="mt-4" onClick={() => router.push("/home")}>
                    Back to Home
                  </Button>
                </div>
              ) : (
                <div className="space-y-6">
                  {providers.map((provider) => (
                    <WorkerCard
                      key={provider.id}
                      worker={provider}
                      onSelect={handleSelectProvider}
                      selected={selectedProvider === provider.id}
                    />
                  ))}
                </div>
              )}
            </CardContent>
            <CardFooter className="flex justify-between">
              <Button variant="outline" onClick={() => router.back()} disabled={isSubmitting}>
                Back
              </Button>
              <Button onClick={handleSubmit} disabled={!selectedProvider || isSubmitting}>
                {isSubmitting ? (
                  <>
                    <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                    Submitting...
                  </>
                ) : (
                  "Confirm Selection"
                )}
              </Button>
            </CardFooter>
          </Card>
        </div>
      </SidebarLayout>
    </ProtectedRoute>
  )
}
