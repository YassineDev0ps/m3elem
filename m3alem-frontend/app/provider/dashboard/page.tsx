"use client"

import { useState, useEffect } from "react"
import ProtectedRoute from "@/components/ProtectedRoute"
import SidebarLayout from "@/components/layouts/SidebarLayout"
import JobRequestCard from "@/components/JobRequestCard"
import { useAuth } from "@/contexts/AuthContext"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Loader2, Star, CheckCircle, Clock, TrendingUp } from "lucide-react"
import { useToast } from "@/components/ui/use-toast"
import { getRequestsByUserId } from "@/lib/mock-service"

interface DashboardData {
  stats: {
    rating: number
    completedJobs: number
    pendingRequests: number
    inProgressJobs: number
  }
  recentRequests: any[]
}

export default function ProviderDashboardPage() {
  const [dashboardData, setDashboardData] = useState<DashboardData | null>(null)
  const [isLoading, setIsLoading] = useState(true)
  const { user } = useAuth()
  const { toast } = useToast()

  useEffect(() => {
    fetchDashboardData()
  }, [])

  const fetchDashboardData = async () => {
    if (!user) return

    setIsLoading(true)
    try {
      // Try to call the API first
      try {
        const response = await fetch("/api/provider/dashboard", {
          headers: {
            Authorization: `Bearer ${user.token}`,
          },
        })

        // Check if the response is JSON
        const contentType = response.headers.get("content-type")
        if (contentType && contentType.includes("application/json")) {
          if (!response.ok) {
            throw new Error("Failed to fetch dashboard data")
          }

          const data = await response.json()
          setDashboardData(data)
          return
        }
        // If we get here, the API isn't available or isn't returning JSON
        throw new Error("API not implemented")
      } catch (error) {
        console.log("API not available, using localStorage fallback")

        // Fall back to localStorage implementation
        // Get requests from localStorage
        const pendingRequests = getRequestsByUserId(user.id, "Provider", "Pending")
        const inProgressRequests = getRequestsByUserId(user.id, "Provider", "InProgress")
        const completedRequests = getRequestsByUserId(user.id, "Provider", "Completed")

        // Get all requests for this provider
        const allRequests = [...pendingRequests, ...inProgressRequests, ...completedRequests]

        // Create mock dashboard data
        const mockDashboardData: DashboardData = {
          stats: {
            rating: 4.7, // Mock rating
            completedJobs: completedRequests.length,
            pendingRequests: pendingRequests.length,
            inProgressJobs: inProgressRequests.length,
          },
          recentRequests: allRequests.slice(0, 5).map((req) => ({
            id: req.id,
            title: req.title,
            description: req.description,
            serviceType: req.serviceType,
            status: req.status,
            createdAt: req.createdAt,
            preferredTime: req.preferredTime,
            location: req.address,
            clientName: "Client Name", // We don't have this in our mock data
            distance: Math.random() * 3, // Random distance
          })),
        }

        setDashboardData(mockDashboardData)
      }
    } catch (error) {
      console.error("Error fetching dashboard data:", error)
      toast({
        variant: "destructive",
        title: "Failed to fetch dashboard data",
        description: "Please try again later.",
      })
    } finally {
      setIsLoading(false)
    }
  }

  const handleAcceptRequest = async (requestId: string) => {
    if (!user) return

    try {
      // Try to call the API first
      try {
        const response = await fetch(`/api/provider/requests/${requestId}/accept`, {
          method: "POST",
          headers: {
            Authorization: `Bearer ${user.token}`,
          },
        })

        // Check if the response is JSON
        const contentType = response.headers.get("content-type")
        if (contentType && contentType.includes("application/json")) {
          if (!response.ok) {
            throw new Error("Failed to accept request")
          }

          toast({
            title: "Request accepted",
            description: "You have successfully accepted the request.",
          })

          // Refresh dashboard data
          fetchDashboardData()
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
                status: "Accepted",
                providerId: user.id,
                updatedAt: new Date().toISOString(),
              }
            }
            return req
          })
          localStorage.setItem("m3alem_requests", JSON.stringify(updatedRequests))
        }

        toast({
          title: "Request accepted (Mock)",
          description: "Using localStorage mock implementation since the API isn't available yet.",
        })

        // Refresh dashboard data
        fetchDashboardData()
      }
    } catch (error) {
      console.error("Error accepting request:", error)
      toast({
        variant: "destructive",
        title: "Failed to accept request",
        description: "Please try again later.",
      })
    }
  }

  const handleDeclineRequest = async (requestId: string) => {
    if (!user) return

    try {
      // Try to call the API first
      try {
        const response = await fetch(`/api/provider/requests/${requestId}/decline`, {
          method: "POST",
          headers: {
            Authorization: `Bearer ${user.token}`,
          },
        })

        // Check if the response is JSON
        const contentType = response.headers.get("content-type")
        if (contentType && contentType.includes("application/json")) {
          if (!response.ok) {
            throw new Error("Failed to decline request")
          }

          toast({
            title: "Request declined",
            description: "You have declined the request. It remains available for other providers.",
          })

          // Refresh dashboard data
          fetchDashboardData()
          return
        }
        // If we get here, the API isn't available or isn't returning JSON
        throw new Error("API not implemented")
      } catch (error) {
        console.log("API not available, using localStorage fallback")

        // Update the request in localStorage - keep it in Pending state but remove this provider
        const existingRequests = localStorage.getItem("m3alem_requests")
        if (existingRequests) {
          const allRequests = JSON.parse(existingRequests)
          const updatedRequests = allRequests.map((req: any) => {
            if (req.id === requestId) {
              // If the provider was assigned to this request, remove them
              if (req.providerId === user.id) {
                return {
                  ...req,
                  providerId: null, // Remove provider assignment
                  status: "Pending", // Keep it in Pending state
                  updatedAt: new Date().toISOString(),
                }
              }
              // If this provider wasn't assigned, no change needed
              return req
            }
            return req
          })
          localStorage.setItem("m3alem_requests", JSON.stringify(updatedRequests))
        }

        toast({
          title: "Request declined (Mock)",
          description: "You have declined the request. It remains available for other providers.",
        })

        // Refresh dashboard data
        fetchDashboardData()
      }
    } catch (error) {
      console.error("Error declining request:", error)
      toast({
        variant: "destructive",
        title: "Failed to decline request",
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

          // Refresh dashboard data
          fetchDashboardData()
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

        // Refresh dashboard data
        fetchDashboardData()
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

          // Refresh dashboard data
          fetchDashboardData()
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

        // Refresh dashboard data
        fetchDashboardData()
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

  return (
    <ProtectedRoute allowedRoles={["Provider"]}>
      <SidebarLayout>
        <div className="space-y-6">
          <div>
            <h1 className="text-3xl font-bold">Provider Dashboard</h1>
            <p className="text-gray-500 mt-2">Manage your service requests and track your performance</p>
          </div>

          {isLoading ? (
            <div className="flex justify-center items-center py-12">
              <Loader2 className="h-8 w-8 animate-spin text-primary" />
              <span className="ml-2">Loading dashboard data...</span>
            </div>
          ) : !dashboardData ? (
            <div className="text-center py-12">
              <p className="text-lg text-gray-500">Failed to load dashboard data. Please try again later.</p>
            </div>
          ) : (
            <>
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
                <Card>
                  <CardContent className="p-6">
                    <div className="flex items-center space-x-2">
                      <div className="p-2 bg-yellow-100 rounded-full">
                        <Star className="h-6 w-6 text-yellow-600" />
                      </div>
                      <div>
                        <p className="text-sm font-medium text-gray-500">Rating</p>
                        <h3 className="text-2xl font-bold">
                          {dashboardData.stats.rating.toFixed(1)}
                          <span className="text-sm font-normal text-gray-500">/5</span>
                        </h3>
                      </div>
                    </div>
                  </CardContent>
                </Card>

                <Card>
                  <CardContent className="p-6">
                    <div className="flex items-center space-x-2">
                      <div className="p-2 bg-green-100 rounded-full">
                        <CheckCircle className="h-6 w-6 text-green-600" />
                      </div>
                      <div>
                        <p className="text-sm font-medium text-gray-500">Completed Jobs</p>
                        <h3 className="text-2xl font-bold">{dashboardData.stats.completedJobs}</h3>
                      </div>
                    </div>
                  </CardContent>
                </Card>

                <Card>
                  <CardContent className="p-6">
                    <div className="flex items-center space-x-2">
                      <div className="p-2 bg-blue-100 rounded-full">
                        <Clock className="h-6 w-6 text-blue-600" />
                      </div>
                      <div>
                        <p className="text-sm font-medium text-gray-500">Pending Requests</p>
                        <h3 className="text-2xl font-bold">{dashboardData.stats.pendingRequests}</h3>
                      </div>
                    </div>
                  </CardContent>
                </Card>

                <Card>
                  <CardContent className="p-6">
                    <div className="flex items-center space-x-2">
                      <div className="p-2 bg-purple-100 rounded-full">
                        <TrendingUp className="h-6 w-6 text-purple-600" />
                      </div>
                      <div>
                        <p className="text-sm font-medium text-gray-500">In Progress</p>
                        <h3 className="text-2xl font-bold">{dashboardData.stats.inProgressJobs}</h3>
                      </div>
                    </div>
                  </CardContent>
                </Card>
              </div>

              <Card>
                <CardHeader>
                  <CardTitle>Recent Requests</CardTitle>
                  <CardDescription>Your most recent service requests</CardDescription>
                </CardHeader>
                <CardContent>
                  {dashboardData.recentRequests.length === 0 ? (
                    <div className="text-center py-6">
                      <p className="text-gray-500">No recent requests found.</p>
                    </div>
                  ) : (
                    <div className="space-y-4">
                      {dashboardData.recentRequests.map((request) => (
                        <JobRequestCard
                          key={request.id}
                          request={request}
                          userRole="Provider"
                          onAccept={handleAcceptRequest}
                          onDecline={handleDeclineRequest}
                          onStartWork={handleStartWork}
                          onComplete={handleCompleteRequest}
                        />
                      ))}
                    </div>
                  )}
                </CardContent>
              </Card>
            </>
          )}
        </div>
      </SidebarLayout>
    </ProtectedRoute>
  )
}
