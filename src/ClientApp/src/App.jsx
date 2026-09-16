import React, { useEffect, useState, useCallback } from 'react'
import { api, getToken, setToken, clearToken } from './api.js'
import Login from './components/Login.jsx'
import Dashboard from './components/Dashboard.jsx'
import Wizard from './components/Wizard.jsx'
import Settings from './components/Settings.jsx'
import Reports from './components/Reports.jsx'

const NAV = [
  { key: 'dashboard', icon: '📊', label: 'داشبورد', desc: 'نمای کلی موجودی انتشار گازهای گلخانه‌ای' },
  { key: 'wizard', icon: '🧮', label: 'ورود داده (گام‌به‌گام)', desc: 'ثبت داده‌های فعالیت به تفکیک دسته انتشار' },
  { key: 'settings', icon: '⚙️', label: 'تنظیمات محاسبات', desc: 'فرمول‌ها، ضرایب انتشار و پارامترها' },
  { key: 'reports', icon: '📑', label: 'گزارش‌ها', desc: 'موجودی انتشار و ردپای کربن محصولات' }
]

export default function App() {
  const [token, setTok] = useState(getToken())
  const [page, setPage] = useState('dashboard')
  const [toast, setToast] = useState(null)

  const notify = useCallback((msg, error = false) => {
    setToast({ msg, error })
    setTimeout(() => setToast(null), 4000)
  }, [])

  useEffect(() => {
    const h = (e) => { e.preventDefault(); notify('برای خروج از دکمه خروج استفاده کنید') }
    window.addEventListener('beforeunload', h)
    return () => window.removeEventListener('beforeunload', h)
  }, [notify])

  if (!token) {
    return (
      <Login onLogin={(t) => { setToken(t); setTok(t); notify('ورود موفق - خوش آمدید') }}
        onError={(m) => notify(m, true)} />
    )
  }

  const active = NAV.find(n => n.key === page)
  return (
    <div className="app">
      <aside className="sidebar">
        <div className="brand">
          <div className="brand-logo">🌿</div>
          <div>
            <div className="brand-title">ردپای کربن MSC</div>
            <div className="brand-sub">GHG Atlas · ISO 14064/14067</div>
          </div>
        </div>
        {NAV.map(n => (
          <div key={n.key} className={`nav-item ${page === n.key ? 'active' : ''}`} onClick={() => setPage(n.key)}>
            <span className="ico">{n.icon}</span> {n.label}
          </div>
        ))}
        <div className="nav-sep" />
        <div className="nav-item" onClick={() => { clearToken(); setTok(null) }}>
          <span className="ico">🚪</span> خروج
        </div>
        <div className="sidebar-footer">
          داده اولیه: ورک‌بوک MSC-GHG<br />آینده: Web Service
        </div>
      </aside>

      <main className="main">
        <div className="topbar">
          <div>
            <div className="page-title">{active.icon} {active.label}</div>
            <div className="page-desc">{active.desc}</div>
          </div>
          <div className="topbar-actions">
            <div className="user-chip"><span className="dot" /> متصل به API</div>
          </div>
        </div>
        {page === 'dashboard' && <Dashboard notify={notify} />}
        {page === 'wizard' && <Wizard notify={notify} />}
        {page === 'settings' && <Settings notify={notify} />}
        {page === 'reports' && <Reports notify={notify} />}
      </main>

      {toast && <div className={`toast ${toast.error ? 'error' : ''}`}>{toast.error ? '⛔ ' : '✅ '}{toast.msg}</div>}
    </div>
  )
}
