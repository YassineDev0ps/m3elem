"use client"

import { useState } from "react"
import { Card, CardContent, CardFooter } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Badge } from "@/components/ui/badge"
import { Calendar, Clock, MapPin, MessageSquare, MoreVertical, CheckCircle, XCircle, Star, Search } from "lucide-react"
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger } from "@/components/ui/dropdown-menu"
import Link from "next/link"

export type RequestStatus = "Pending" | "Accepted" | "InProgress" | "Completed" | "Cancelled" | "Declined"

interface JobRequestCardProps {
  request: {
    id: string
    title: string
    description: string
    serviceType: string
    status: RequestStatus
    createdAt: string
    preferredTime: string
    location: string
    distance?: number
    clientName?: string
    providerName?: string
    rated?: boolean
  }
  userRole: "Seeker" | "Provider"
  onAccept?: (id: string) => void
  onDecline?: (id: string) => void
  onCancel?: (id: string) => void
  onStartWork?: (id: string) => void
  onComplete?: (id: string) => void
}

export default function JobRequestCard({
  request,
  userRole,
  onAccept,
  onDecline,
  onCancel,
  onStartWork,
  onComplete,
}: JobRequestCardProps) {
  const [isLoading, setIsLoading] = useState(false)

  const formatDate = (dateString: string) => {
    const date = new Date(dateString)
    return date.toLocaleDateString("en-US", {
      year: "numeric",
      month: "short",
      day: "numeric",
    })
  }

  const getStatusColor = (status: RequestStatus) => {
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

  const handleAction = async (action: "accept" | "decline" | "cancel" | "startWork" | "complete") => {
    setIsLoading(true)
    try {
      switch (action) {
        case "accept":
          onAccept && (await onAccept(request.id))
          break
        case "decline":
          onDecline && (await onDecline(request.id))
          break
        case "cancel":
          onCancel && (await onCancel(request.id))
          break
        case "startWork":
          onStartWork && (await onStartWork(request.id))
          break
        case "complete":
          onComplete && (await onComplete(request.id))
          break
      }
    } catch (error) {
      console.error(`Error during ${action} action:`, error)
    } finally {
      setIsLoading(false)
    }
  }

  return (
    <Card className="overflow-hidden">
      <CardContent className="p-6">
        <div className="flex justify-between items-start">
          <div>
            <div className="flex items-center gap-2">
              <h3 className="font-semibold text-lg">{request.title}</h3>
              <Badge className={getStatusColor(request.status)}>{request.status}</Badge>
            </div>

            {userRole === "Provider" && request.clientName && (
              <p className="text-sm text-gray-500 mt-1">Requested by: {request.clientName}</p>
            )}

            {userRole === "Seeker" && request.providerName && (
              <p className="text-sm text-gray-500 mt-1">Provider: {request.providerName}</p>
            )}
          </div>

          <DropdownMenu>
            <DropdownMenuTrigger asChild>
              <Button variant="ghost" size="icon">
                <MoreVertical className="h-4 w-4" />
                <span className="sr-only">Menu</span>
              </Button>
            </DropdownMenuTrigger>
            <DropdownMenuContent align="end">
              <DropdownMenuItem asChild>
                <Link href={`/chat/${request.id}`}>
                  <MessageSquare className="mr-2 h-4 w-4" />
                  <span>Message</span>
                </Link>
              </DropdownMenuItem>

              {userRole === "Seeker" && request.status === "Pending" && (
                <DropdownMenuItem asChild>
                  <Link href={`/search-providers?requestId=${request.id}`}>
                    <Search className="mr-2 h-4 w-4" />
                    <span>Find Providers</span>
                  </Link>
                </DropdownMenuItem>
              )}

              {userRole === "Seeker" && request.status === "Completed" && !request.rated && (
                <DropdownMenuItem asChild>
                  <Link href={`/rate/${request.id}`}>
                    <Star className="mr-2 h-4 w-4" />
                    <span>Rate Provider</span>
                  </Link>
                </DropdownMenuItem>
              )}

              {userRole === "Seeker" && request.status === "Pending" && (
                <DropdownMenuItem onClick={() => handleAction("cancel")}>
                  <XCircle className="mr-2 h-4 w-4" />
                  <span>Cancel Request</span>
                </DropdownMenuItem>
              )}

              {userRole === "Provider" && request.status === "Pending" && (
                <>
                  <DropdownMenuItem onClick={() => handleAction("accept")}>
                    <CheckCircle className="mr-2 h-4 w-4" />
                    <span>Accept Request</span>
                  </DropdownMenuItem>
                  <DropdownMenuItem onClick={() => handleAction("decline")}>
                    <XCircle className="mr-2 h-4 w-4" />
                    <span>Decline Request</span>
                  </DropdownMenuItem>
                </>
              )}

              {userRole === "Provider" && request.status === "Accepted" && (
                <DropdownMenuItem onClick={() => handleAction("startWork")}>
                  <Clock className="mr-2 h-4 w-4" />
                  <span>Start Work</span>
                </DropdownMenuItem>
              )}

              {userRole === "Provider" && request.status === "InProgress" && (
                <DropdownMenuItem onClick={() => handleAction("complete")}>
                  <CheckCircle className="mr-2 h-4 w-4" />
                  <span>Mark as Completed</span>
                </DropdownMenuItem>
              )}

              {userRole === "Seeker" && (request.status === "Pending" || request.status === "Accepted") && (
                <DropdownMenuItem onClick={() => handleAction("cancel")}>
                  <XCircle className="mr-2 h-4 w-4" />
                  <span>Cancel Request</span>
                </DropdownMenuItem>
              )}
            </DropdownMenuContent>
          </DropdownMenu>
        </div>

        <p className="text-gray-600 mt-3 text-sm line-clamp-2">{request.description}</p>

        <div className="mt-4 space-y-2">
          <div className="flex items-center text-sm text-gray-500">
            <Badge variant="outline" className="mr-2">
              {request.serviceType}
            </Badge>
          </div>

          <div className="flex items-center text-sm text-gray-500">
            <Calendar className="h-4 w-4 mr-2" />
            <span>Created on {formatDate(request.createdAt)}</span>
          </div>

          <div className="flex items-center text-sm text-gray-500">
            <Clock className="h-4 w-4 mr-2" />
            <span>Preferred time: {request.preferredTime}</span>
          </div>

          <div className="flex items-center text-sm text-gray-500">
            <MapPin className="h-4 w-4 mr-2" />
            <span>{request.location}</span>
            {request.distance !== undefined && (
              <span className="ml-1">
                (
                {request.distance < 1
                  ? `${(request.distance * 1000).toFixed(0)}m away`
                  : `${request.distance.toFixed(1)}km away`}
                )
              </span>
            )}
          </div>
        </div>
      </CardContent>

      <CardFooter className="bg-gray-50 px-6 py-3 flex justify-between">
        {/* Message button for non-pending requests or providers */}
        {(request.status !== "Pending" || userRole === "Provider") && (
          <Link href={`/chat/${request.id}`}>
            <Button variant="outline" size="sm">
              <MessageSquare className="mr-2 h-4 w-4" />
              Message
            </Button>
          </Link>
        )}

        {/* Rate provider button for completed requests */}
        {userRole === "Seeker" && request.status === "Completed" && !request.rated && (
          <Link href={`/rate/${request.id}`}>
            <Button size="sm">
              <Star className="mr-2 h-4 w-4" />
              Rate Provider
            </Button>
          </Link>
        )}

        {/* Provider actions for pending requests */}
        {userRole === "Provider" && request.status === "Pending" && (
          <div className="space-x-2">
            <Button variant="destructive" size="sm" onClick={() => handleAction("decline")} disabled={isLoading}>
              Decline
            </Button>
            <Button size="sm" onClick={() => handleAction("accept")} disabled={isLoading}>
              Accept
            </Button>
          </div>
        )}

        {/* Provider action for accepted requests */}
        {userRole === "Provider" && request.status === "Accepted" && (
          <Button size="sm" onClick={() => handleAction("startWork")} disabled={isLoading}>
            Start Work
          </Button>
        )}

        {/* Provider action for in-progress requests */}
        {userRole === "Provider" && request.status === "InProgress" && (
          <Button size="sm" onClick={() => handleAction("complete")} disabled={isLoading}>
            Mark Complete
          </Button>
        )}

        {/* Seeker actions for pending requests */}
        {userRole === "Seeker" && request.status === "Pending" && (
          <div className="flex justify-between w-full">
            <Link href={`/search-providers?requestId=${request.id}`}>
              <Button variant="outline" size="sm">
                <Search className="mr-2 h-4 w-4" />
                Find Providers
              </Button>
            </Link>
            <Button variant="destructive" size="sm" onClick={() => handleAction("cancel")} disabled={isLoading}>
              Cancel Request
            </Button>
          </div>
        )}

        {/* Seeker cancel action for accepted requests */}
        {userRole === "Seeker" && request.status === "Accepted" && (
          <Button variant="destructive" size="sm" onClick={() => handleAction("cancel")} disabled={isLoading}>
            Cancel Request
          </Button>
        )}
      </CardFooter>
    </Card>
  )
}
