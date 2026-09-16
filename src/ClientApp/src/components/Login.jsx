import React, { useState } from 'react'
import { api } from '../api.js'

export default function Login({ onLogin, onError }) {
  const [username, setUsername] = useState('Admin')
  const [password, setPassword] = useState('')
  const [busy, setBusy] = useState(false)

  const submit = async (e) => {
    e.preventDefault()
    setBusy(true)
    try {
      const res = await api.login(username.trim(), password)
      if (!res?.AccessToken) throw new Error('ورود ناموفق - توکن دریافت نشد (محیط Development لازم است)')
      onLogin(res.AccessToken)
    } catch (err) {
      onError(err.message)
    } finally {
      setBusy(false)
    }
  }

  return (
    <div className="login-wrap">
      <form className="card login-card" onSubmit={submit}>
        <div className="login-logo">🌿</div>
        <div className="login-title">سامانه مدیریت ردپای کربن</div>
        <div className="login-sub">مجتمع فولاد مبارکه · ISO 14064-1 / ISO 14067</div>
        <input className="input" placeholder="نام کاربری" value={username} onChange={e => setUsername(e.target.value)} dir="ltr" />
        <input className="input" type="password" placeholder="رمز عبور" value={password} onChange={e => setPassword(e.target.value)} dir="ltr" />
        <button className="btn primary" style={{ width: '100%', justifyContent: 'center', padding: 13 }} disabled={busy}>
          {busy ? '⏳ در حال ورود...' : 'ورود به سامانه'}
        </button>
        <div className="hint" style={{ marginTop: 16 }}>
          ورود آزمایشی توسعه (LoginDevelop) · کاربر پیش‌فرض: Admin / Admin@123
        </div>
      </form>
    </div>
  )
}
