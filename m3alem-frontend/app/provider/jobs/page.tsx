"use client"

import { useState, useEffect } from "react"
import ProtectedRoute from "@/components/ProtectedRoute"
import SidebarLayout from "@/components/layouts/SidebarLayout"
import JobRequestCard, { type RequestStatus } from "@/components/JobRequestCard"
import { useAuth } from "@/contexts/AuthContext"
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"
import { Loader2 } from "lucide-react"
import { useToast } from "@/components/ui/use-toast"
import { getRequestsByUserId } from "@/lib/mock-service"

interface Request {
  id: string
  title: string
  description: string
  serviceType: string
  status: RequestStatus
  createdAt: string
  preferredTime: string
  location: string
  clientName: string
  distance: number
}

export default function ProviderJobsPage() {
  const [requests, setRequests] = useState<Request[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [activeTab, setActiveTab] = useState<string>("Accepted")
  const { user } = useAuth()
  const { toast } = useToast()

  useEffect(() => {
    fetchRequests(activeTab)
  }, [activeTab])

  const fetchRequests = async (status: string) => {
    if (!user) return

    setIsLoading(true)
    try {
      // Try to call the API first
      try {
        const response = await fetch(`/api/provider/requests?status=${status}`, {
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
        const userRequests = getRequestsByUserId(user.id, "Provider", status)

        if (userRequests.length > 0) {
          // Map the requests to the format expected by the component
          const formattedRequests = userRequests.map((req: any) => ({
            id: req.id,
            title: req.title,
            description: req.description,
            serviceType: req.serviceType,
            status: req.status as RequestStatus,
            createdAt: req.createdAt,
            preferredTime: req.preferredTime,
            location: req.address,
            clientName: "Client Name", // We don't have this in our mock data
            distance: Math.random() * 3, // Random distance
          }))

          setRequests(formattedRequests)
        } else {
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

  const handleStartWork = async (requestId: string) => {
    if (!user) return

    try {
      // Try to call the API first
      try {
        const response = await fetch(`/api/provider/requests/${requestId}/start`, {
          method: "POST",
          headers: {
            Authorization: `Bearer ${user.token}`,
          },
        })

        // Check if the response is JSON
        const contentType = response.headers.get("content-type")
        if (contentType && contentType.includes("application/json")) {
          if (!response.ok) {
            throw new Error("Failed to start work on request")
          }

          toast({
            title: "Work started",
            description: "You have started working on this request.",
          })

          // Refresh requests
          fetchRequests(activeTab)
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
                status: "InProgress",
                updatedAt: new Date().toISOString(),
              }
            }
            return req
          })
          localStorage.setItem("m3alem_requests", JSON.stringify(updatedRequests))
        }

        toast({
          title: "Work started (Mock)",
          description: "Using localStorage mock implementation since the API isn't available yet.",
        })

        // Refresh requests
        fetchRequests(activeTab)
      }
    } catch (error) {
      console.error("Error starting work on request:", error)
      toast({
        variant: "destructive",
        title: "Failed to start work",
        description: "Please try again later.",
      })
    }
  }

  const handleCompleteRequest = async (requestId: string) => {
    if (!user) return

    try {
      // Try to call the API first
      try {
        const response = await fetch(`/api/provider/requests/${requestId}/complete`, {
          method: "POST",
          headers: {
            Authorization: `Bearer ${user.token}`,
          },
        })

        // Check if the response is JSON
        const contentType = response.headers.get("content-type")
        if (contentType && contentType.includes("application/json")) {
          if (!response.ok) {
            throw new Error("Failed to complete request")
          }

          toast({
            title: "Request completed",
            description: "You have marked the request as completed.",
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
                status: "Completed",
                updatedAt: new Date().toISOString(),
              }
            }
            return req
          })
          localStorage.setItem("m3alem_requests", JSON.stringify(updatedRequests))
        }

        toast({
          title: "Request completed (Mock)",
          description: "Using localStorage mock implementation since the API isn't available yet.",
        })

        // Update the local state
        setRequests(requests.filter((req) => req.id !== requestId))
      }
    } catch (error) {
      console.error("Error completing request:", error)
      toast({
        variant: "destructive",
        title: "Failed to complete request",
        description: "Please try again later.",
      })
    }
  }

  return (
    <ProtectedRoute allowedRoles={["Provider"]}>
      <SidebarLayout>
        <div className="space-y-6">
          <div>
            <h1 className="text-3xl font-bold">My Jobs</h1>
            <p className="text-gray-500 mt-2">Manage and track your service jobs</p>
          </div>

          <Tabs defaultValue="Accepted" onValueChange={setActiveTab}>
            <TabsList className="grid grid-cols-3 mb-6">
              <TabsTrigger value="Accepted">Accepted</TabsTrigger>
              <TabsTrigger value="InProgress">In Progress</TabsTrigger>
              <TabsTrigger value="Completed">Completed</TabsTrigger>
            </TabsList>

            {["Accepted", "InProgress", "Completed"].map((status) => (
              <TabsContent key={status} value={status} className="space-y-4">
                {isLoading ? (
                  <div className="flex justify-center items-center py-12">
                    <Loader2 className="h-8 w-8 animate-spin text-primary" />
                    <span className="ml-2">Loading jobs...</span>
                  </div>
                ) : requests.length === 0 ? (
                  <div className="text-center py-12">
                    <p className="text-lg text-gray-500">No {status.toLowerCase()} jobs found.</p>
                  </div>
                ) : (
                  requests.map((request) => (
                    <JobRequestCard
                      key={request.id}
                      request={request}
                      userRole="Provider"
                      onStartWork={status === "Accepted" ? handleStartWork : undefined}
                      onComplete={status === "InProgress" ? handleCompleteRequest : undefined}
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
