"use client"
import { Card, CardContent, CardFooter } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Badge } from "@/components/ui/badge"
import { Star, MapPin, Clock, CheckCircle } from "lucide-react"

interface WorkerCardProps {
  worker: {
    id: string
    fullName: string
    rating: number
    reviewCount: number
    distance: number
    skills: string[]
    availability: string[]
    experience: number
    completedJobs: number
  }
  onSelect?: (workerId: string) => void
  selected?: boolean
}

export default function WorkerCard({ worker, onSelect, selected = false }: WorkerCardProps) {
  const formatSkills = (skills: string[]) => {
    return skills.map((skill) => skill.charAt(0).toUpperCase() + skill.slice(1).replace("_", " "))
  }

  const formatAvailability = (availability: string[]) => {
    const formatted = availability.map((a) => {
      const parts = a.split("_")
      return `${parts[0].charAt(0).toUpperCase() + parts[0].slice(1)} ${parts[1].charAt(0).toUpperCase() + parts[1].slice(1)}`
    })

    return formatted.join(", ")
  }

  return (
    <Card className={`overflow-hidden transition-all ${selected ? "ring-2 ring-primary" : ""}`}>
      <CardContent className="p-6">
        <div className="flex items-start justify-between">
          <div className="flex items-center">
            <div className="h-12 w-12 rounded-full bg-primary/20 flex items-center justify-center text-primary font-bold text-xl">
              {worker.fullName.charAt(0)}
            </div>
            <div className="ml-4">
              <h3 className="font-semibold text-lg">{worker.fullName}</h3>
              <div className="flex items-center mt-1">
                <div className="flex items-center">
                  {[...Array(5)].map((_, i) => (
                    <Star
                      key={i}
                      className={`h-4 w-4 ${
                        i < Math.floor(worker.rating) ? "text-yellow-400 fill-yellow-400" : "text-gray-300"
                      }`}
                    />
                  ))}
                  <span className="ml-1 text-sm font-medium">{worker.rating.toFixed(1)}</span>
                </div>
                <span className="mx-2 text-gray-300">•</span>
                <span className="text-sm text-gray-500">{worker.reviewCount} reviews</span>
              </div>
            </div>
          </div>
          <div className="flex items-center text-sm text-gray-500">
            <MapPin className="h-4 w-4 mr-1" />
            {worker.distance < 1
              ? `${(worker.distance * 1000).toFixed(0)}m away`
              : `${worker.distance.toFixed(1)}km away`}
          </div>
        </div>

        <div className="mt-4">
          <div className="flex flex-wrap gap-1 mb-3">
            {formatSkills(worker.skills).map((skill, index) => (
              <Badge key={index} variant="secondary" className="font-normal">
                {skill}
              </Badge>
            ))}
          </div>

          <div className="flex items-center text-sm text-gray-500 mb-2">
            <Clock className="h-4 w-4 mr-2" />
            <span>{formatAvailability(worker.availability)}</span>
          </div>

          <div className="flex items-center text-sm text-gray-500 mb-2">
            <CheckCircle className="h-4 w-4 mr-2" />
            <span>{worker.completedJobs} jobs completed</span>
          </div>

          <div className="text-sm text-gray-500">
            <span className="font-medium">{worker.experience}</span> years of experience
          </div>
        </div>
      </CardContent>

      <CardFooter className="bg-gray-50 px-6 py-3">
        <Button
          className="w-full"
          variant={selected ? "secondary" : "default"}
          onClick={() => onSelect && onSelect(worker.id)}
        >
          {selected ? "Selected" : "Select Provider"}
        </Button>
      </CardFooter>
    </Card>
  )
}
