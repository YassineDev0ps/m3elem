"use client"

import { useState, useEffect } from "react"
import ProtectedRoute from "@/components/ProtectedRoute"
import SidebarLayout from "@/components/layouts/SidebarLayout"
import JobRequestCard, { type RequestStatus } from "@/components/JobRequestCard"
import { useAuth } from "@/contexts/AuthContext"
import { Button } from "@/components/ui/button"
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"
import { Loader2 } from "lucide-react"
import { useToast } from "@/components/ui/use-toast"
import { getRequestsByUserId, getUserById } from "@/lib/mock-service"

interface Request {
  id: string
  title: string
  description: string
  serviceType: string
  status: RequestStatus
  createdAt: string
  preferredTime: string
  location: string
  providerName?: string
  rated?: boolean
}

export default function MyRequestsPage() {
  const [requests, setRequests] = useState<Request[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [activeTab, setActiveTab] = useState<RequestStatus>("Pending")
  const { user } = useAuth()
  const { toast } = useToast()

  useEffect(() => {
    if (user) {
      fetchRequests(activeTab)
    }
  }, [activeTab, user])

  const fetchRequests = async (status: RequestStatus) => {
    if (!user) return

    setIsLoading(true)
    try {
      // Try to call the API first
      try {
        const response = await fetch(`/api/seeker/requests?status=${status}`, {
          headers: {
            Authorization: `Bearer ${user.token}`,
          },
        })

        // Check if the response is JSON
        const contentType = response.headers.get("content-type")
        if (contentType && contentType.includes("application/json")) {
          if (!response.ok) {
            throw new Error("Failed to fetch requests")
          }

          const data = await response.json()
          setRequests(data)
          return
        }
        // If we get here, the API isn't available or isn't returning JSON
        throw new Error("API not implemented")
      } catch (error) {
        console.log("API not available, using localStorage fallback")

        // Get requests from localStorage using the mock service
        const userRequests = getRequestsByUserId(user.id, "Seeker", status)
        console.log("User requests:", userRequests)

        if (userRequests.length > 0) {
          // Map the requests to the format expected by the component
          const formattedRequests = userRequests.map((req: any) => {
            // Get provider name if available
            let providerName = ""
            if (req.providerId) {
              const provider = getUserById(req.providerId)
              if (provider) {
                providerName = provider.fullName
              }
            }

            return {
              id: req.id,
              title: req.title,
              description: req.description,
              serviceType: req.serviceType,
              status: req.status as RequestStatus,
              createdAt: req.createdAt,
              preferredTime: req.preferredTime,
              location: req.address,
              providerName: providerName,
              rated: req.rated || false,
            }
          })

          console.log("Formatted requests:", formattedRequests)
          setRequests(formattedRequests)
        } else {
          console.log("No requests found for this user and status")
          setRequests([])
        }
      }
    } catch (error) {
      console.error("Error fetching requests:", error)
      toast({
        variant: "destructive",
        title: "Failed to fetch requests",
        description: "Please try again later.",
      })
    } finally {
      setIsLoading(false)
    }
  }

  const handleCancelRequest = async (requestId: string) => {
    if (!user) return

    try {
      // Try to call the API first
      try {
        const response = await fetch(`/api/seeker/requests/${requestId}/cancel`, {
          method: "PUT",
          headers: {
            Authorization: `Bearer ${user.token}`,
          },
        })

        // Check if the response is JSON
        const contentType = response.headers.get("content-type")
        if (contentType && contentType.includes("application/json")) {
          if (!response.ok) {
            throw new Error("Failed to cancel request")
          }

          toast({
            title: "Request cancelled",
            description: "Your request has been cancelled successfully.",
          })

          // Update the local state
          setRequests(requests.filter((req) => req.id !== requestId))
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
                status: "Cancelled",
                updatedAt: new Date().toISOString(),
              }
            }
            return req
          })
          localStorage.setItem("m3alem_requests", JSON.stringify(updatedRequests))
        }

        toast({
          title: "Request cancelled (Mock)",
          description: "Your request has been cancelled successfully.",
        })

        // Update the local state
        setRequests(requests.filter((req) => req.id !== requestId))
      }
    } catch (error) {
      console.error("Error cancelling request:", error)
      toast({
        variant: "destructive",
        title: "Failed to cancel request",
        description: "Please try again later.",
      })
    }
  }

  return (
    <ProtectedRoute allowedRoles={["Seeker"]}>
      <SidebarLayout>
        <div className="space-y-6">
          <div>
            <h1 className="text-3xl font-bold">My Requests</h1>
            <p className="text-gray-500 mt-2">Manage and track your service requests</p>
          </div>

          <Tabs defaultValue="Pending" onValueChange={(value) => setActiveTab(value as RequestStatus)}>
            <TabsList className="grid grid-cols-4 mb-6">
              <TabsTrigger value="Pending">Pending</TabsTrigger>
              <TabsTrigger value="Accepted">Accepted</TabsTrigger>
              <TabsTrigger value="InProgress">In Progress</TabsTrigger>
              <TabsTrigger value="Completed">Completed</TabsTrigger>
            </TabsList>

            {["Pending", "Accepted", "InProgress", "Completed"].map((status) => (
              <TabsContent key={status} value={status} className="space-y-4">
                {isLoading ? (
                  <div className="flex justify-center items-center py-12">
                    <Loader2 className="h-8 w-8 animate-spin text-primary" />
                    <span className="ml-2">Loading requests...</span>
                  </div>
                ) : requests.length === 0 ? (
                  <div className="text-center py-12">
                    <p className="text-lg text-gray-500">No {status.toLowerCase()} requests found.</p>
                    {status === "Pending" && (
                      <Button className="mt-4" onClick={() => (window.location.href = "/home")}>
                        Create New Request
                      </Button>
                    )}
                  </div>
                ) : (
                  requests.map((request) => (
                    <JobRequestCard
                      key={request.id}
                      request={request}
                      userRole="Seeker"
                      onCancel={handleCancelRequest}
                    />
                  ))
                )}
              </TabsContent>
            ))}
          </Tabs>
        </div>
      </SidebarLayout>
    </ProtectedRoute>
  )
}
