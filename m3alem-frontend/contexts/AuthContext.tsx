"use client"

import { createContext, useContext, useState, useEffect, type ReactNode } from "react"
import { useRouter } from "next/navigation"
import { useToast } from "@/components/ui/use-toast"

export type UserRole = "Seeker" | "Provider"

interface User {
  id: string
  fullName: string
  role: UserRole
  token: string
}

interface AuthContextType {
  user: User | null
  isLoading: boolean
  login: (email: string, password: string) => Promise<void>
  logout: () => Promise<void>
  registerSeeker: (userData: any) => Promise<void>
  registerProvider: (userData: any) => Promise<void>
  isAuthenticated: boolean
}

const AuthContext = createContext<AuthContextType | undefined>(undefined)

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  const [user, setUser] = useState<User | null>(null)
  const [isLoading, setIsLoading] = useState(true)
  const router = useRouter()
  const { toast } = useToast()

  useEffect(() => {
    // Check for stored auth token on initial load
    const storedUser = localStorage.getItem("m3alem_user")
    if (storedUser) {
      try {
        const parsedUser = JSON.parse(storedUser)
        setUser(parsedUser)
      } catch (error) {
        console.error("Failed to parse stored user data", error)
        localStorage.removeItem("m3alem_user")
      }
    }
    setIsLoading(false)
  }, [])

  const login = async (email: string, password: string) => {
    setIsLoading(true)
    try {
      // Try to call the API first
      try {
        const response = await fetch("/api/auth/login", {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify({ email, password }),
        })

        // Check if the response is JSON
        const contentType = response.headers.get("content-type")
        if (contentType && contentType.includes("application/json")) {
          if (!response.ok) {
            const error = await response.json()
            throw new Error(error.message || "Login failed")
          }

          const userData = await response.json()
          setUser(userData)
          localStorage.setItem("m3alem_user", JSON.stringify(userData))

          // Redirect based on user role
          if (userData.role === "Seeker") {
            router.push("/home")
          } else {
            router.push("/provider/dashboard")
          }

          toast({
            title: "Login successful",
            description: `Welcome back, ${userData.fullName}!`,
          })
          return
        }
        // If we get here, the API isn't available or isn't returning JSON
        throw new Error("API not implemented")
      } catch (error) {
        console.log("API not available, using localStorage fallback")

        // Fall back to localStorage implementation
        const existingUsers = localStorage.getItem("m3alem_users")
        if (!existingUsers) {
          throw new Error("Invalid email or password")
        }

        const users = JSON.parse(existingUsers)
        const user = users.find((u: any) => u.email === email)

        if (!user) {
          throw new Error("Invalid email or password")
        }

        // Note: In a real app, we'd hash passwords, but for this mock we can't verify password
        // since we don't store it. Just simulate successful login always.

        const loggedInUser = {
          id: user.id,
          fullName: user.fullName,
          email: user.email,
          role: user.role,
          token: `mock-token-${Math.random().toString(36).substring(2, 15)}`,
        }

        setUser(loggedInUser)
        localStorage.setItem("m3alem_user", JSON.stringify(loggedInUser))

        // Redirect based on user role
        if (user.role === "Seeker") {
          router.push("/home")
        } else {
          router.push("/provider/dashboard")
        }

        toast({
          title: "Login successful (Mock)",
          description: `Welcome back, ${user.fullName}! Using localStorage mock implementation since the API isn't available yet.`,
        })
      }
    } catch (error) {
      toast({
        variant: "destructive",
        title: "Login failed",
        description: error instanceof Error ? error.message : "An error occurred during login",
      })
      throw error
    } finally {
      setIsLoading(false)
    }
  }

  const logout = async () => {
    setIsLoading(true)
    try {
      // Try to call the API first
      try {
        const response = await fetch("/api/auth/logout", {
          method: "POST",
          headers: {
            Authorization: `Bearer ${user?.token}`,
          },
        })

        if (response.ok) {
          setUser(null)
          localStorage.removeItem("m3alem_user")
          router.push("/")

          toast({
            title: "Logged out successfully",
          })
          return
        }
        throw new Error("API not implemented")
      } catch (error) {
        // Fall back to localStorage implementation
        console.log("API not available, using localStorage fallback")

        setUser(null)
        localStorage.removeItem("m3alem_user")
        router.push("/")

        toast({
          title: "Logged out successfully (Mock)",
          description: "Using localStorage mock implementation since the API isn't available yet.",
        })
      }
    } catch (error) {
      console.error("Logout error:", error)
    } finally {
      setIsLoading(false)
    }
  }

  const registerSeeker = async (userData: any) => {
    setIsLoading(true)
    try {
      // Try to call the API first
      try {
        const response = await fetch("/api/auth/register/seeker", {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify(userData),
        })

        // Check if the response is JSON
        const contentType = response.headers.get("content-type")
        if (contentType && contentType.includes("application/json")) {
          if (!response.ok) {
            const error = await response.json()
            throw new Error(error.message || "Registration failed")
          }

          const newUser = await response.json()
          setUser(newUser)
          localStorage.setItem("m3alem_user", JSON.stringify(newUser))

          router.push("/home")

          toast({
            title: "Registration successful",
            description: "Your account has been created successfully!",
          })
          return
        }
        // If we get here, the API isn't available or isn't returning JSON
        throw new Error("API not implemented")
      } catch (error) {
        console.log("API not available, using localStorage fallback")
        // Fall back to localStorage implementation
        // Generate a mock user with a mock token
        const mockUser = {
          id: `seeker-${Date.now()}`,
          fullName: userData.fullName,
          email: userData.email,
          role: "Seeker" as UserRole,
          token: `mock-token-${Math.random().toString(36).substring(2, 15)}`,
        }

        // Store the user data in localStorage
        const existingUsers = localStorage.getItem("m3alem_users")
        const users = existingUsers ? JSON.parse(existingUsers) : []

        // Check if the email is already used
        const emailExists = users.some((user: any) => user.email === userData.email)
        if (emailExists) {
          throw new Error("Email already exists")
        }

        // Store complete user data for mock functionality
        users.push({
          ...mockUser,
          ...userData,
          password: undefined, // Don't store password in plain text
        })
        localStorage.setItem("m3alem_users", JSON.stringify(users))

        // Set the current user
        setUser(mockUser)
        localStorage.setItem("m3alem_user", JSON.stringify(mockUser))

        router.push("/home")

        toast({
          title: "Registration successful (Mock)",
          description: "Using localStorage mock implementation since the API isn't available yet.",
        })
      }
    } catch (error) {
      toast({
        variant: "destructive",
        title: "Registration failed",
        description: error instanceof Error ? error.message : "An error occurred during registration",
      })
      console.error("Registration error:", error)
      throw error
    } finally {
      setIsLoading(false)
    }
  }

  const registerProvider = async (userData: any) => {
    setIsLoading(true)
    try {
      // Try to call the API first
      try {
        const response = await fetch("/api/auth/register/provider", {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify(userData),
        })

        // Check if the response is JSON
        const contentType = response.headers.get("content-type")
        if (contentType && contentType.includes("application/json")) {
          if (!response.ok) {
            const error = await response.json()
            throw new Error(error.message || "Registration failed")
          }

          const newUser = await response.json()
          setUser(newUser)
          localStorage.setItem("m3alem_user", JSON.stringify(newUser))

          router.push("/provider/dashboard")

          toast({
            title: "Registration successful",
            description: "Your provider account has been created successfully!",
          })
          return
        }
        // If we get here, the API isn't available or isn't returning JSON
        throw new Error("API not implemented")
      } catch (error) {
        console.log("API not available, using localStorage fallback")
        // Fall back to localStorage implementation
        // Generate a mock user with a mock token
        const mockUser = {
          id: `provider-${Date.now()}`,
          fullName: userData.fullName,
          email: userData.email,
          role: "Provider" as UserRole,
          token: `mock-token-${Math.random().toString(36).substring(2, 15)}`,
        }

        // Store the user data in localStorage
        const existingUsers = localStorage.getItem("m3alem_users")
        const users = existingUsers ? JSON.parse(existingUsers) : []

        // Check if the email is already used
        const emailExists = users.some((user: any) => user.email === userData.email)
        if (emailExists) {
          throw new Error("Email already exists")
        }

        // Store complete user data for mock functionality
        users.push({
          ...mockUser,
          ...userData,
          password: undefined, // Don't store password in plain text
        })
        localStorage.setItem("m3alem_users", JSON.stringify(users))

        // Set the current user
        setUser(mockUser)
        localStorage.setItem("m3alem_user", JSON.stringify(mockUser))

        router.push("/provider/dashboard")

        toast({
          title: "Registration successful (Mock)",
          description: "Using localStorage mock implementation since the API isn't available yet.",
        })
      }
    } catch (error) {
      toast({
        variant: "destructive",
        title: "Registration failed",
        description: error instanceof Error ? error.message : "An error occurred during registration",
      })
      console.error("Registration error:", error)
      throw error
    } finally {
      setIsLoading(false)
    }
  }

  return (
    <AuthContext.Provider
      value={{
        user,
        isLoading,
        login,
        logout,
        registerSeeker,
        registerProvider,
        isAuthenticated: !!user,
      }}
    >
      {children}
    </AuthContext.Provider>
  )
}

export const useAuth = () => {
  const context = useContext(AuthContext)
  if (context === undefined) {
    throw new Error("useAuth must be used within an AuthProvider")
  }
  return context
}
