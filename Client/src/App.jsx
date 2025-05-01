import { useState } from 'react'
import reactLogo from './assets/react.svg'
import viteLogo from '/vite.svg'
import './index.css';
import './App.css'
import { Import } from 'lucide-react';

function App() {
  const [count, setCount] = useState(0)

  return (

  <div className="min-h-screen flex items-center justify-center bg-gray-100">
      <div className="text-center">
        <h1 className="text-4xl font-semibold mb-4 text-blue-600">Vite + React + Tailwind CSS</h1>
        <div className="bg-white shadow-md rounded-lg p-6">
          <button 
            onClick={() => setCount(count + 1)} 
            className="px-4 py-2 bg-blue-500 text-white rounded hover:bg-blue-600 transition">
            Count is {count}
          </button>
          <p className="mt-4 text-sm text-gray-600">
            Edit <code>src/App.jsx</code> and save to test hot module replacement (HMR)
          </p>
        </div>
      </div>
    </div>
  )

}

export default App
