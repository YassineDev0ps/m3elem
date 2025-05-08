"use client"

import { useState, useEffect } from "react"
import { useParams } from "next/navigation"
import ProtectedRoute from "@/components/ProtectedRoute"
import SidebarLayout from "@/components/layouts/SidebarLayout"
import ChatBox from "@/components/ChatBox"
import { useAuth } from "@/contexts/AuthContext"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Badge } from "@/components/ui/badge"
import { Loader2 } from "lucide-react"
import { useToast } from "@/components/ui/use-toast"
import { getRequestById, getUserById } from "@/lib/mock-service"

interface RequestDetails {
  id: string
  title: string
  status: string
  serviceType: string
  otherUserName: string
  otherUserId: string
}

export default function ChatPage() {
  const params = useParams()
  const requestId = params.requestId as string
  const { user } = useAuth()
  const { toast } = useToast()

  const [requestDetails, setRequestDetails] = useState<RequestDetails | null>(null)
  const [isLoading, setIsLoading] = useState(true)

  useEffect(() => {
    fetchRequestDetails()
  }, [requestId])

  const fetchRequestDetails = async () => {
    if (!user || !requestId) return

    setIsLoading(true)
    try {
      // Try to call the API first
      try {
        const response = await fetch(`/api/requests/${requestId}/details`, {
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
            description: "The requested chat could not be found.",
          })
          return
        }

        // Determine the other user based on the current user's role
        let otherUserId = ""
        let otherUserName = ""

        if (user.role === "Seeker") {
          // If current user is a seeker, the other user is the provider
          otherUserId = request.providerId || ""
          if (otherUserId) {
            const provider = getUserById(otherUserId)
            otherUserName = provider ? provider.fullName : "Service Provider"
          } else {
            otherUserName = "Service Provider"
          }
        } else {
          // If current user is a provider, the other user is the seeker
          otherUserId = request.seekerId || ""
          if (otherUserId) {
            const seeker = getUserById(otherUserId)
            otherUserName = seeker ? seeker.fullName : "Client"
          } else {
            otherUserName = "Client"
          }
        }

        setRequestDetails({
          id: request.id,
          title: request.title,
          status: request.status,
          serviceType: request.serviceType,
          otherUserName: otherUserName,
          otherUserId: otherUserId,
        })
      }
    } catch (error) {
      console.error("Error fetching request details:", error)
      toast({
        variant: "destructive",
        title: "Failed to fetch request details",
        description: "Please try again later.",
      })
    } finally {
      setIsLoading(false)
    }
  }

  const getStatusColor = (status: string) => {
    switch (status) {
      case "Pending":
        return "bg-yellow-100 text-yellow-800"
      case "Accepted":
        return "bg-blue-100 text-blue-800"
      case "InProgress":
        return "bg-purple-100 text-purple-800"
      case "Completed":
        return "bg-green-100 text-green-800"
      case "Cancelled":
        return "bg-red-100 text-red-800"
      case "Declined":
        return "bg-gray-100 text-gray-800"
      default:
        return "bg-gray-100 text-gray-800"
    }
  }

  return (
    <ProtectedRoute>
      <SidebarLayout>
        <div className="space-y-6">
          <div>
            <h1 className="text-3xl font-bold">Messages</h1>
            <p className="text-gray-500 mt-2">Communicate with your service provider</p>
          </div>

          {isLoading ? (
            <div className="flex justify-center items-center py-12">
              <Loader2 className="h-8 w-8 animate-spin text-primary" />
              <span className="ml-2">Loading chat...</span>
            </div>
          ) : !requestDetails ? (
            <div className="text-center py-12">
              <p className="text-lg text-gray-500">Chat not found or you don't have permission to access it.</p>
            </div>
          ) : (
            <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
              <div className="md:col-span-2">
                <ChatBox requestId={requestId} otherUserName={requestDetails.otherUserName} />
              </div>

              <div>
                <Card>
                  <CardHeader>
                    <CardTitle>Request Details</CardTitle>
                    <CardDescription>Information about this service request</CardDescription>
                  </CardHeader>
                  <CardContent className="space-y-4">
                    <div>
                      <h3 className="font-medium">Title</h3>
                      <p>{requestDetails.title}</p>
                    </div>

                    <div>
                      <h3 className="font-medium">Service Type</h3>
                      <Badge variant="outline">{requestDetails.serviceType}</Badge>
                    </div>

                    <div>
                      <h3 className="font-medium">Status</h3>
                      <Badge className={getStatusColor(requestDetails.status)}>{requestDetails.status}</Badge>
                    </div>

                    <div>
                      <h3 className="font-medium">{user?.role === "Seeker" ? "Provider" : "Client"}</h3>
                      <p>{requestDetails.otherUserName}</p>
                    </div>
                  </CardContent>
                </Card>
              </div>
            </div>
          )}
        </div>
      </SidebarLayout>
    </ProtectedRoute>
  )
}
