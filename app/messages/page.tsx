"use client"

import { useState, useEffect } from "react"
import { useRouter } from "next/navigation"
import { useAuth } from "@/contexts/AuthContext"
import { useToast } from "@/components/ui/use-toast"

interface ChatThread {
  requestId: string
  requestTitle: string
  otherUserName: string
  lastMessage: string
  lastMessageTime: string
  unreadCount: number
}

export default function MessagesPage() {
  const [chatThreads, setChatThreads] = useState<ChatThread[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const { user } = useAuth();
  const { toast } = useToast();
  const router = useRouter();

  useEffect(() => {
    fetchChatThreads();
  }, []);

  const fetchChatThreads = async () => {
    if (!user) return;

    setIsLoading(true);
    try {
      const response = await fetch("/api/messages", {
        headers: {
          Authorization: Bearer ${user.token},
        },
      });

      if (!response.ok) {
        throw new Error("Failed to fetch chat threads");
      }

      const data = await response.json();
      setChatThreads(data);
    } catch (error) {
      toast({
        title: "Error",
        description: "Failed to load messages",
        variant: "destructive",
      });
    } finally {
      setIsLoading(false);
    }
  };
}