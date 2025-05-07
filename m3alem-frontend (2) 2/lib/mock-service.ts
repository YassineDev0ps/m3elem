// This file provides mock service functions to handle data in localStorage

// Helper function to generate a unique ID
export const generateId = (prefix: string) => `${prefix}-${Date.now()}-${Math.random().toString(36).substring(2, 9)}`

// User related functions
export const getUsers = () => {
  const users = localStorage.getItem("m3alem_users")
  return users ? JSON.parse(users) : []
}

export const saveUser = (user: any) => {
  const users = getUsers()
  users.push(user)
  localStorage.setItem("m3alem_users", JSON.stringify(users))
}

export const getUserById = (id: string) => {
  const users = getUsers()
  return users.find((user: any) => user.id === id)
}

export const getUserByEmail = (email: string) => {
  const users = getUsers()
  return users.find((user: any) => user.email === email)
}

// Request related functions
export const getRequests = () => {
  const requests = localStorage.getItem("m3alem_requests")
  return requests ? JSON.parse(requests) : []
}

export const saveRequest = (request: any) => {
  const requests = getRequests()
  requests.push(request)
  localStorage.setItem("m3alem_requests", JSON.stringify(requests))
  return request
}

export const getRequestById = (id: string) => {
  const requests = getRequests()
  return requests.find((request: any) => request.id === id)
}

export const updateRequest = (id: string, updates: any) => {
  const requests = getRequests()
  const updatedRequests = requests.map((request: any) => {
    if (request.id === id) {
      return { ...request, ...updates, updatedAt: new Date().toISOString() }
    }
    return request
  })
  localStorage.setItem("m3alem_requests", JSON.stringify(updatedRequests))
  return updatedRequests.find((request: any) => request.id === id)
}

export const getRequestsByUserId = (userId: string, role: string, status?: string) => {
  const requests = getRequests()
  return requests.filter((request: any) => {
    if (role === "Seeker") {
      const isUserRequest = request.seekerId === userId
      return status ? isUserRequest && request.status === status : isUserRequest
    } else if (role === "Provider") {
      const isUserRequest = request.providerId === userId
      return status ? isUserRequest && request.status === status : isUserRequest
    }
    return false
  })
}

// Provider related functions
export const getProviders = () => {
  const users = getUsers()
  return users.filter((user: any) => user.role === "Provider")
}

export const getRandomProviders = (count = 3) => {
  const providers = getProviders()
  // If we don't have enough providers, return what we have
  if (providers.length <= count) return providers

  // Otherwise, return a random selection
  const shuffled = [...providers].sort(() => 0.5 - Math.random())
  return shuffled.slice(0, count)
}

// Mock data initialization
export const initializeMockData = () => {
  // Check if we already have data
  if (getUsers().length > 0) return

  // Create some mock providers
  const mockProviders = [
    {
      id: "provider-1",
      fullName: "Ahmed Benani",
      email: "ahmed@example.com",
      role: "Provider",
      phone: "+212612345678",
      bio: "Professional plumber with 5 years of experience",
      experience: 5,
      skills: ["plumbing", "electrical"],
      availability: ["weekday_morning", "weekend_afternoon"],
      latitude: 31.7917,
      longitude: -7.0926,
      address: "123 Main St, Casablanca",
      rating: 4.8,
      reviewCount: 24,
      completedJobs: 42,
    },
    {
      id: "provider-2",
      fullName: "Fatima Zahra",
      email: "fatima@example.com",
      role: "Provider",
      phone: "+212623456789",
      bio: "Experienced cleaner and gardener",
      experience: 3,
      skills: ["cleaning", "gardening"],
      availability: ["weekday_afternoon", "weekend_morning"],
      latitude: 31.78,
      longitude: -7.08,
      address: "456 Park Ave, Casablanca",
      rating: 4.5,
      reviewCount: 18,
      completedJobs: 27,
    },
    {
      id: "provider-3",
      fullName: "Mohammed Alaoui",
      email: "mohammed@example.com",
      role: "Provider",
      phone: "+212634567890",
      bio: "Master carpenter and painter with 8 years of experience",
      experience: 8,
      skills: ["carpentry", "painting"],
      availability: ["weekday_evening", "weekend_evening"],
      latitude: 31.8,
      longitude: -7.1,
      address: "789 Oak St, Casablanca",
      rating: 4.9,
      reviewCount: 36,
      completedJobs: 65,
    },
  ]

  // Save mock providers
  mockProviders.forEach((provider) => {
    const users = getUsers()
    users.push(provider)
    localStorage.setItem("m3alem_users", JSON.stringify(users))
  })

  console.log("Mock data initialized")
}
