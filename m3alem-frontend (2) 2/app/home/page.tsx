"use client"

import { useState } from "react"
import { useRouter } from "next/navigation"
import ProtectedRoute from "@/components/ProtectedRoute"
import SidebarLayout from "@/components/layouts/SidebarLayout"
import ServiceSelector from "@/components/ServiceSelector"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"

export default function HomePage() {
  const [selectedService, setSelectedService] = useState<string | null>(null)
  const router = useRouter()

  const handleServiceSelect = (serviceId: string) => {
    setSelectedService(serviceId)
  }

  const handleContinue = () => {
    if (selectedService) {
      router.push(`/create-request?service=${selectedService}`)
    }
  }

  return (
    <ProtectedRoute allowedRoles={["Seeker"]}>
      <SidebarLayout>
        <div className="space-y-6">
          <div>
            <h1 className="text-3xl font-bold">Find a Service Provider</h1>
            <p className="text-gray-500 mt-2">Select a service category to get started</p>
          </div>

          <Card>
            <CardHeader>
              <CardTitle>What service do you need?</CardTitle>
              <CardDescription>Browse through our available service categories</CardDescription>
            </CardHeader>
            <CardContent>
              <ServiceSelector onSelect={handleServiceSelect} />

              <div className="mt-6 flex justify-end">
                <Button onClick={handleContinue} disabled={!selectedService} size="lg">
                  Continue
                </Button>
              </div>
            </CardContent>
          </Card>

          <div className="grid md:grid-cols-3 gap-6">
            <Card>
              <CardHeader>
                <CardTitle className="flex items-center">
                  <span className="bg-primary/10 text-primary p-2 rounded-full mr-2">1</span>
                  Select a Service
                </CardTitle>
              </CardHeader>
              <CardContent>
                <p className="text-gray-500">
                  Choose from our wide range of home services provided by skilled professionals.
                </p>
              </CardContent>
            </Card>

            <Card>
              <CardHeader>
                <CardTitle className="flex items-center">
                  <span className="bg-primary/10 text-primary p-2 rounded-full mr-2">2</span>
                  Describe Your Need
                </CardTitle>
              </CardHeader>
              <CardContent>
                <p className="text-gray-500">
                  Tell us what you need done, when you need it, and provide your location details.
                </p>
              </CardContent>
            </Card>

            <Card>
              <CardHeader>
                <CardTitle className="flex items-center">
                  <span className="bg-primary/10 text-primary p-2 rounded-full mr-2">3</span>
                  Choose a Provider
                </CardTitle>
              </CardHeader>
              <CardContent>
                <p className="text-gray-500">
                  Review profiles, ratings, and select the best match for your job from our list of qualified providers.
                </p>
              </CardContent>
            </Card>
          </div>
        </div>
      </SidebarLayout>
    </ProtectedRoute>
  )
}
