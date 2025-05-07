"use client"

import { useState, useEffect } from "react"
import { useParams, useRouter } from "next/navigation"
import ProtectedRoute from "@/components/ProtectedRoute"
import SidebarLayout from "@/components/layouts/SidebarLayout"
import { useAuth } from "@/contexts/AuthContext"
import { Button } from "@/components/ui/button"
import { Textarea } from "@/components/ui/textarea"
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from "@/components/ui/card"
import { Star, Loader2 } from "lucide-react"
import { useToast } from "@/components/ui/use-toast"
import { getRequestById, getUserById } from "@/lib/mock-service"

interface RequestDetails {
  id: string
  title: string
  providerName: string
  serviceType: string
}

export default function RatePage() {
  const params = useParams()
  const requestId = params.requestId as string
  const router = useRouter()
  const { user } = useAuth()
  const { toast } = useToast()

  const [requestDetails, setRequestDetails] = useState<RequestDetails | null>(null)
  const [rating, setRating] = useState(0)
  const [hoverRating, setHoverRating] = useState(0)
  const [comment, setComment] = useState("")
  const [isLoading, setIsLoading] = useState(true)
  const [isSubmitting, setIsSubmitting] = useState(false)

  useEffect(() => {
    fetchRequestDetails()
  }, [requestId])

  const fetchRequestDetails = async () => {
    if (!user || !requestId) return

    setIsLoading(true)
    try {
      // Try to call the API first
      try {
        const response = await fetch(`/api/seeker/requests/${requestId}`, {
          headers: {
            Authorization: `Bearer ${user.token}`,
          },
        })

        // Check if the response is JSON
        const contentType = response.headers.get("content-type")
        if (contentType && contentType.includes("application/json")) {
          if (!response.ok) {
            throw new Error("Failed to fetch request details")
          }

          const data = await response.json()
          setRequestDetails(data)
          return
        }
        // If we get here, the API isn't available or isn't returning JSON
        throw new Error("API not implemented")
      } catch (error) {
        console.log("API not available, using localStorage fallback")

        // Get request from localStorage
        const request = getRequestById(requestId)

        if (!request) {
          toast({
            variant: "destructive",
            title: "Request not found",
            description: "The requested job could not be found.",
          })
          router.push("/my-requests")
          return
        }

        if (request.status !== "Completed") {
          toast({
            variant: "destructive",
            title: "Cannot rate this request",
            description: "You can only rate completed requests.",
          })
          router.push("/my-requests")
          return
        }

        // Get provider name if available
        let providerName = "Service Provider"
        if (request.providerId) {
          const provider = getUserById(request.providerId)
          if (provider) {
            providerName = provider.fullName
          }
        }

        setRequestDetails({
          id: request.id,
          title: request.title,
          providerName: providerName,
          serviceType: request.serviceType,
        })
      }
    } catch (error) {
      console.error("Error fetching request details:", error)
      toast({
        variant: "destructive",
        title: "Failed to fetch request details",
        description: "Please try again later.",
      })
      router.push("/my-requests")
    } finally {
      setIsLoading(false)
    }
  }

  const handleSubmit = async () => {
    if (rating === 0) {
      toast({
        variant: "destructive",
        title: "Rating required",
        description: "Please select a rating before submitting.",
      })
      return
    }

    if (!user || !requestId) return

    setIsSubmitting(true)
    try {
      // Try to call the API first
      try {
        const response = await fetch(`/api/seeker/requests/${requestId}/rate`, {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${user.token}`,
          },
          body: JSON.stringify({
            rating,
            comment,
          }),
        })

        // Check if the response is JSON
        const contentType = response.headers.get("content-type")
        if (contentType && contentType.includes("application/json")) {
          if (!response.ok) {
            throw new Error("Failed to submit rating")
          }

          toast({
            title: "Rating submitted",
            description: "Thank you for your feedback!",
          })

          router.push("/my-requests?tab=Completed")
          return
        }
        // If we get here, the API isn't available or isn't returning JSON
        throw new Error("API not implemented")
      } catch (error) {
        console.log("API not available, using localStorage fallback")

        // Update the request in localStorage
        const existingRequests = localStorage.getItem("m3alem_requests")
        if (existingRequests) {
          const allRequests = JSON.parse(existingRequests)
          const updatedRequests = allRequests.map((req: any) => {
            if (req.id === requestId) {
              return {
                ...req,
                rated: true,
                rating: rating,
                ratingComment: comment,
                ratedAt: new Date().toISOString(),
              }
            }
            return req
          })
          localStorage.setItem("m3alem_requests", JSON.stringify(updatedRequests))
        }

        toast({
          title: "Rating submitted (Mock)",
          description: "Thank you for your feedback! Using localStorage mock implementation.",
        })

        router.push("/my-requests?tab=Completed")
      }
    } catch (error) {
      console.error("Error submitting rating:", error)
      toast({
        variant: "destructive",
        title: "Failed to submit rating",
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
            <h1 className="text-3xl font-bold">Rate Service Provider</h1>
            <p className="text-gray-500 mt-2">Share your experience with the service provider</p>
          </div>

          {isLoading ? (
            <div className="flex justify-center items-center py-12">
              <Loader2 className="h-8 w-8 animate-spin text-primary" />
              <span className="ml-2">Loading request details...</span>
            </div>
          ) : !requestDetails ? (
            <div className="text-center py-12">
              <p className="text-lg text-gray-500">Request not found or you don't have permission to rate it.</p>
              <Button className="mt-4" onClick={() => router.push("/my-requests")}>
                Back to My Requests
              </Button>
            </div>
          ) : (
            <Card>
              <CardHeader>
                <CardTitle>Rate your experience</CardTitle>
                <CardDescription>
                  How was your experience with {requestDetails.providerName} for "{requestDetails.title}"?
                </CardDescription>
              </CardHeader>
              <CardContent className="space-y-6">
                <div className="flex justify-center">
                  <div className="flex space-x-2">
                    {[1, 2, 3, 4, 5].map((star) => (
                      <button
                        key={star}
                        type="button"
                        className="focus:outline-none"
                        onClick={() => setRating(star)}
                        onMouseEnter={() => setHoverRating(star)}
                        onMouseLeave={() => setHoverRating(0)}
                      >
                        <Star
                          className={`h-10 w-10 ${
                            star <= (hoverRating || rating) ? "text-yellow-400 fill-yellow-400" : "text-gray-300"
                          }`}
                        />
                      </button>
                    ))}
                  </div>
                </div>

                <div className="text-center">
                  <p className="text-lg font-medium">
                    {rating === 1 && "Poor"}
                    {rating === 2 && "Fair"}
                    {rating === 3 && "Good"}
                    {rating === 4 && "Very Good"}
                    {rating === 5 && "Excellent"}
                  </p>
                </div>

                <div className="space-y-2">
                  <label htmlFor="comment" className="text-sm font-medium">
                    Additional comments (optional)
                  </label>
                  <Textarea
                    id="comment"
                    placeholder="Share details of your experience..."
                    value={comment}
                    onChange={(e) => setComment(e.target.value)}
                    rows={4}
                  />
                </div>
              </CardContent>
              <CardFooter className="flex justify-between">
                <Button variant="outline" onClick={() => router.push("/my-requests")} disabled={isSubmitting}>
                  Cancel
                </Button>
                <Button onClick={handleSubmit} disabled={rating === 0 || isSubmitting}>
                  {isSubmitting ? (
                    <>
                      <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                      Submitting...
                    </>
                  ) : (
                    "Submit Rating"
                  )}
                </Button>
              </CardFooter>
            </Card>
          )}
        </div>
      </SidebarLayout>
    </ProtectedRoute>
  )
}
