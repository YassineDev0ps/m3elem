"use client"

import { useState } from "react"
import { useRouter } from "next/navigation"
import { Card, CardContent } from "@/components/ui/card"
import { cn } from "@/lib/utils"

interface Service {
  id: string
  name: string
  icon: string
  description: string
}

interface ServiceSelectorProps {
  onSelect?: (serviceId: string) => void
  redirectOnSelect?: boolean
}

export default function ServiceSelector({ onSelect, redirectOnSelect = false }: ServiceSelectorProps) {
  const [selectedService, setSelectedService] = useState<string | null>(null)
  const router = useRouter()

  const services: Service[] = [
    {
      id: "plumbing",
      name: "Plumbing",
      icon: "🔧",
      description: "Pipe repairs, installations, and drainage solutions",
    },
    {
      id: "electrical",
      name: "Electrical",
      icon: "⚡",
      description: "Wiring, fixtures, and electrical system repairs",
    },
    {
      id: "carpentry",
      name: "Carpentry",
      icon: "🪚",
      description: "Furniture repair, woodwork, and installations",
    },
    {
      id: "painting",
      name: "Painting",
      icon: "🖌️",
      description: "Interior and exterior painting services",
    },
    {
      id: "cleaning",
      name: "Cleaning",
      icon: "🧹",
      description: "Home and office cleaning services",
    },
    {
      id: "moving",
      name: "Moving",
      icon: "📦",
      description: "Furniture moving and relocation assistance",
    },
    {
      id: "gardening",
      name: "Gardening",
      icon: "🌱",
      description: "Garden maintenance, planting, and landscaping",
    },
    {
      id: "appliance_repair",
      name: "Appliance Repair",
      icon: "🔌",
      description: "Repair services for household appliances",
    },
  ]

  const handleSelect = (serviceId: string) => {
    setSelectedService(serviceId)

    if (onSelect) {
      onSelect(serviceId)
    }

    if (redirectOnSelect) {
      router.push(`/create-request?service=${serviceId}`)
    }
  }

  return (
    <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
      {services.map((service) => (
        <Card
          key={service.id}
          className={cn(
            "cursor-pointer transition-all hover:shadow-md",
            selectedService === service.id
              ? "border-primary bg-primary/5 ring-2 ring-primary"
              : "hover:border-primary/50",
          )}
          onClick={() => handleSelect(service.id)}
        >
          <CardContent className="p-4 text-center">
            <div className="text-4xl mb-2">{service.icon}</div>
            <h3 className="font-medium text-lg">{service.name}</h3>
            <p className="text-sm text-gray-500 mt-1">{service.description}</p>
          </CardContent>
        </Card>
      ))}
    </div>
  )
}
