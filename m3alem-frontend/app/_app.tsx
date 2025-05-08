"use client"

// This file initializes the mock data when the app starts
import { useEffect } from "react"
import { initializeMockData } from "@/lib/mock-service"

export default function App({ Component, pageProps }) {
  useEffect(() => {
    // Initialize mock data on app start
    initializeMockData()
  }, [])

  return <Component {...pageProps} />
}
