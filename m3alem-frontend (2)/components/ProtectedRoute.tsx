"use client"

import type React from "react"

import { useAuth, type UserRole } from "@/contexts/AuthContext"
import { useRouter, usePathname } from "next/navigation"
import { useEffect } from "react"
import { Loader2 } from "lucide-react"

interface ProtectedRouteProps {
  children: React.ReactNode
  allowedRoles?: UserRole[]
}

export default function ProtectedRoute({ children, allowedRoles }: ProtectedRouteProps) {
  const { user, isLoading, isAuthenticated } = useAuth()
  const router = useRouter()
  const pathname = usePathname()

  useEffect(() => {
    if (!isLoading && !isAuthenticated) {
      // Store the attempted URL to redirect back after login
      sessionStorage.setItem("redirectAfterLogin", pathname)
      router.push("/login")
      return
    }

    if (!isLoading && isAuthenticated && allowedRoles) {
      const userRole = user?.role as UserRole
      if (!allowedRoles.includes(userRole)) {
        // Redirect to appropriate home based on role
        if (userRole === "Seeker") {
          router.push("/home")
        } else {
          router.push("/provider/dashboard")
        }
      }
    }
  }, [isLoading, isAuthenticated, user, router, pathname, allowedRoles])

  if (isLoading) {
    return (
      <div className="flex h-screen w-full items-center justify-center">
        <Loader2 className="h-8 w-8 animate-spin text-primary" />
        <span className="ml-2 text-lg">Loading...</span>
      </div>
    )
  }

  if (!isAuthenticated) {
    return null // Will redirect in useEffect
  }

  if (allowedRoles && user) {
    if (!allowedRoles.includes(user.role)) {
      return null // Will redirect in useEffect
    }
  }

  return <>{children}</>
}
