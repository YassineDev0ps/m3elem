"use client"

import type React from "react"

import { useState, type ReactNode } from "react"
import Link from "next/link"
import { usePathname } from "next/navigation"
import { useAuth, type UserRole } from "@/contexts/AuthContext"
import { Home, User, Briefcase, MessageSquare, LogOut, Menu, X } from "lucide-react"
import { Button } from "@/components/ui/button"
import { cn } from "@/lib/utils"
import { Sidebar, SidebarContent, SidebarFooter, SidebarHeader, SidebarProvider } from "@/components/ui/sidebar"

interface SidebarLayoutProps {
  children: ReactNode
}

interface NavItem {
  label: string
  href: string
  icon: React.ElementType
  roles: UserRole[]
}

export default function SidebarLayout({ children }: SidebarLayoutProps) {
  const { user, logout } = useAuth()
  const pathname = usePathname()
  const [mobileOpen, setMobileOpen] = useState(false)

  const navItems: NavItem[] = [
    // Seeker routes
    {
      label: "Home",
      href: "/home",
      icon: Home,
      roles: ["Seeker"],
    },
    {
      label: "My Requests",
      href: "/my-requests",
      icon: Briefcase,
      roles: ["Seeker"],
    },

    // Provider routes
    {
      label: "Dashboard",
      href: "/provider/dashboard",
      icon: Home,
      roles: ["Provider"],
    },
    {
      label: "Jobs",
      href: "/provider/jobs",
      icon: Briefcase,
      roles: ["Provider"],
    },

    // Shared routes
    {
      label: "Profile",
      href: user?.role === "Seeker" ? "/profile" : "/provider/profile",
      icon: User,
      roles: ["Seeker", "Provider"],
    },
    {
      label: "Messages",
      href: "/messages",
      icon: MessageSquare,
      roles: ["Seeker", "Provider"],
    },
  ]

  const filteredNavItems = navItems.filter((item) => user && item.roles.includes(user.role as UserRole))

  const closeMobileMenu = () => {
    setMobileOpen(false)
  }

  return (
    <SidebarProvider>
      <div className="flex min-h-screen bg-gray-50">
        {/* Desktop Sidebar */}
        <Sidebar className="hidden md:flex">
          <SidebarHeader className="p-4 border-b">
            <Link href="/" className="flex items-center space-x-2">
              <div className="bg-primary rounded-md p-1">
                <span className="text-white font-bold text-xl">M3</span>
              </div>
              <span className="font-bold text-xl">M3alem.ma</span>
            </Link>
          </SidebarHeader>

          <SidebarContent className="py-4">
            <nav className="space-y-1 px-2">
              {filteredNavItems.map((item) => (
                <Link
                  key={item.href}
                  href={item.href}
                  className={cn(
                    "flex items-center px-4 py-3 text-sm font-medium rounded-md transition-colors",
                    pathname === item.href ? "bg-primary text-white" : "text-gray-700 hover:bg-gray-100",
                  )}
                >
                  <item.icon className="mr-3 h-5 w-5" />
                  {item.label}
                </Link>
              ))}
            </nav>
          </SidebarContent>

          <SidebarFooter className="p-4 border-t">
            <div className="flex items-center justify-between">
              <div className="flex items-center">
                <div className="h-8 w-8 rounded-full bg-primary flex items-center justify-center text-white">
                  {user?.fullName?.charAt(0) || "U"}
                </div>
                <div className="ml-2">
                  <p className="text-sm font-medium">{user?.fullName}</p>
                  <p className="text-xs text-gray-500">{user?.role}</p>
                </div>
              </div>
              <Button variant="ghost" size="icon" onClick={() => logout()} title="Logout">
                <LogOut className="h-5 w-5" />
              </Button>
            </div>
          </SidebarFooter>
        </Sidebar>

        {/* Mobile Header */}
        <div className="fixed top-0 left-0 right-0 z-10 bg-white border-b md:hidden">
          <div className="flex items-center justify-between p-4">
            <Link href="/" className="flex items-center space-x-2">
              <div className="bg-primary rounded-md p-1">
                <span className="text-white font-bold text-xl">M3</span>
              </div>
              <span className="font-bold text-xl">M3alem.ma</span>
            </Link>
            <Button variant="ghost" size="icon" onClick={() => setMobileOpen(!mobileOpen)}>
              {mobileOpen ? <X className="h-6 w-6" /> : <Menu className="h-6 w-6" />}
            </Button>
          </div>
        </div>

        {/* Mobile Menu */}
        {mobileOpen && (
          <div className="fixed inset-0 z-50 bg-white md:hidden pt-16">
            <nav className="p-4 space-y-2">
              {filteredNavItems.map((item) => (
                <Link
                  key={item.href}
                  href={item.href}
                  className={cn(
                    "flex items-center px-4 py-3 text-base font-medium rounded-md transition-colors",
                    pathname === item.href ? "bg-primary text-white" : "text-gray-700 hover:bg-gray-100",
                  )}
                  onClick={closeMobileMenu}
                >
                  <item.icon className="mr-3 h-5 w-5" />
                  {item.label}
                </Link>
              ))}
              <Button
                variant="ghost"
                className="w-full justify-start px-4 py-3 text-base font-medium rounded-md text-red-600 hover:bg-red-50"
                onClick={() => {
                  logout()
                  closeMobileMenu()
                }}
              >
                <LogOut className="mr-3 h-5 w-5" />
                Logout
              </Button>
            </nav>
          </div>
        )}

        {/* Main Content */}
        <div className="flex-1 flex flex-col md:ml-64">
          <main className="flex-1 p-4 md:p-6 pt-20 md:pt-6">{children}</main>
        </div>
      </div>
    </SidebarProvider>
  )
}
