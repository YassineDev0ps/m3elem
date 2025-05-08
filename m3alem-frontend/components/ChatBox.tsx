"use client"

import type React from "react"

import { useState, useEffect, useRef } from "react"
import { useAuth } from "@/contexts/AuthContext"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Loader2, Send } from "lucide-react"
import { useToast } from "@/components/ui/use-toast"

interface Message {
  id: string
  senderId: string
  content: string
  timestamp: string
  senderName: string
}

interface ChatBoxProps {
  requestId: string
  otherUserName: string
  initialMessages?: Message[]
  pollingInterval?: number
}

export default function ChatBox({
  requestId,
  otherUserName,
  initialMessages = [],
  pollingInterval = 5000,
}: ChatBoxProps) {
  const [messages, setMessages] = useState<Message[]>(initialMessages)
  const [newMessage, setNewMessage] = useState("")
  const [isLoading, setIsLoading] = useState(false)
  const [isSending, setIsSending] = useState(false)
  const { user } = useAuth()
  const { toast } = useToast()
  const messagesEndRef = useRef<HTMLDivElement>(null)
  const chatContainerRef = useRef<HTMLDivElement>(null)

  // Fetch messages on component mount and set up polling
  useEffect(() => {
    fetchMessages()

    const intervalId = setInterval(fetchMessages, pollingInterval)

    return () => clearInterval(intervalId)
  }, [requestId])

  // Scroll to bottom when messages change
  useEffect(() => {
    scrollToBottom()
  }, [messages])

  // Update the fetchMessages function to handle non-JSON responses
  const fetchMessages = async () => {
    if (!user) return

    try {
      const response = await fetch(`/api/messages/${requestId}`, {
        headers: {
          Authorization: `Bearer ${user.token}`,
        },
      })

      // Check if the response is JSON
      const contentType = response.headers.get("content-type")
      if (contentType && contentType.includes("application/json")) {
        if (!response.ok) {
          throw new Error("Failed to fetch messages")
        }

        const data = await response.json()
        setMessages(data)
        return
      }

      // If we get here, the API isn't available or isn't returning JSON
      console.log("API not available, using localStorage fallback for messages")

      // Create mock messages if none exist yet
      const mockMessages = localStorage.getItem(`m3alem_messages_${requestId}`)
      if (mockMessages) {
        setMessages(JSON.parse(mockMessages))
      } else {
        // Initialize with welcome message if no messages exist
        const initialMessage = {
          id: Date.now().toString(),
          senderId: "system",
          content: `Welcome to your conversation! You can start chatting with ${otherUserName} about your service request.`,
          timestamp: new Date().toISOString(),
          senderName: "System",
        }
        setMessages([initialMessage])
        localStorage.setItem(`m3alem_messages_${requestId}`, JSON.stringify([initialMessage]))
      }
    } catch (error) {
      console.error("Error fetching messages:", error)
    } finally {
      setIsLoading(false)
    }
  }

  // Update the sendMessage function to handle non-JSON responses
  const sendMessage = async (e: React.FormEvent) => {
    e.preventDefault()

    if (!newMessage.trim() || !user) return

    setIsSending(true)

    try {
      // Create the message object for optimistic update
      const tempMessage: Message = {
        id: Date.now().toString(),
        senderId: user.id,
        content: newMessage,
        timestamp: new Date().toISOString(),
        senderName: user.fullName,
      }

      // Add optimistic update
      setMessages((prev) => [...prev, tempMessage])
      setNewMessage("")

      // Try to call the API first
      try {
        const response = await fetch(`/api/messages/${requestId}`, {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${user.token}`,
          },
          body: JSON.stringify({ content: newMessage }),
        })

        // Check if the response is JSON
        const contentType = response.headers.get("content-type")
        if (contentType && contentType.includes("application/json")) {
          if (!response.ok) {
            throw new Error("Failed to send message")
          }

          // Fetch the latest messages to ensure consistency
          fetchMessages()
          return
        }

        // If we get here, the API isn't available or isn't returning JSON
        console.log("API not available, using localStorage fallback for sending message")

        // Update localStorage with the new message
        const existingMessages = localStorage.getItem(`m3alem_messages_${requestId}`)
        const messages = existingMessages ? JSON.parse(existingMessages) : []
        messages.push(tempMessage)
        localStorage.setItem(`m3alem_messages_${requestId}`, JSON.stringify(messages))
      } catch (error) {
        console.error("Error sending message to API:", error)

        // Update localStorage with the new message even if API fails
        const existingMessages = localStorage.getItem(`m3alem_messages_${requestId}`)
        const messages = existingMessages ? JSON.parse(existingMessages) : []
        messages.push(tempMessage)
        localStorage.setItem(`m3alem_messages_${requestId}`, JSON.stringify(messages))
      }
    } catch (error) {
      console.error("Error sending message:", error)
      toast({
        variant: "destructive",
        title: "Failed to send message",
        description: "Please try again later.",
      })
    } finally {
      setIsSending(false)
    }
  }

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: "smooth" })
  }

  const formatTime = (timestamp: string) => {
    const date = new Date(timestamp)
    return date.toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" })
  }

  const formatDate = (timestamp: string) => {
    const date = new Date(timestamp)
    return date.toLocaleDateString([], {
      year: "numeric",
      month: "short",
      day: "numeric",
    })
  }

  // Group messages by date
  const groupedMessages: { [date: string]: Message[] } = {}
  messages.forEach((message) => {
    const date = formatDate(message.timestamp)
    if (!groupedMessages[date]) {
      groupedMessages[date] = []
    }
    groupedMessages[date].push(message)
  })

  return (
    <div className="flex flex-col h-full border rounded-lg overflow-hidden bg-white">
      {/* Chat header */}
      <div className="p-4 border-b bg-gray-50">
        <h3 className="font-medium">Chat with {otherUserName}</h3>
      </div>

      {/* Messages container */}
      <div
        ref={chatContainerRef}
        className="flex-1 overflow-y-auto p-4 space-y-4"
        style={{ maxHeight: "calc(100vh - 200px)" }}
      >
        {isLoading && messages.length === 0 ? (
          <div className="flex justify-center items-center h-full">
            <Loader2 className="h-8 w-8 animate-spin text-primary" />
          </div>
        ) : messages.length === 0 ? (
          <div className="flex justify-center items-center h-full text-gray-500">
            <p>No messages yet. Start the conversation!</p>
          </div>
        ) : (
          Object.entries(groupedMessages).map(([date, dateMessages]) => (
            <div key={date} className="space-y-4">
              <div className="flex justify-center">
                <span className="text-xs bg-gray-100 text-gray-500 px-2 py-1 rounded-full">{date}</span>
              </div>

              {dateMessages.map((message) => {
                const isCurrentUser = message.senderId === user?.id

                return (
                  <div key={message.id} className={`flex ${isCurrentUser ? "justify-end" : "justify-start"}`}>
                    <div
                      className={`max-w-[75%] rounded-lg px-4 py-2 ${
                        isCurrentUser
                          ? "bg-primary text-white rounded-br-none"
                          : "bg-gray-100 text-gray-800 rounded-bl-none"
                      }`}
                    >
                      {!isCurrentUser && <p className="text-xs font-medium mb-1">{message.senderName}</p>}
                      <p className="break-words">{message.content}</p>
                      <p
                        className={`text-xs mt-1 text-right ${
                          isCurrentUser ? "text-primary-foreground/80" : "text-gray-500"
                        }`}
                      >
                        {formatTime(message.timestamp)}
                      </p>
                    </div>
                  </div>
                )
              })}
            </div>
          ))
        )}
        <div ref={messagesEndRef} />
      </div>

      {/* Message input */}
      <div className="p-4 border-t">
        <form onSubmit={sendMessage} className="flex space-x-2">
          <Input
            value={newMessage}
            onChange={(e) => setNewMessage(e.target.value)}
            placeholder="Type your message..."
            disabled={isSending}
            className="flex-1"
          />
          <Button type="submit" disabled={isSending || !newMessage.trim()}>
            {isSending ? <Loader2 className="h-4 w-4 animate-spin" /> : <Send className="h-4 w-4" />}
          </Button>
        </form>
      </div>
    </div>
  )
}
