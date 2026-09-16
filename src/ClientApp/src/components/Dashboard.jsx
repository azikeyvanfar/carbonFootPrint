import React, { useEffect, useState } from 'react'
import {
  PieChart, Pie, Cell, BarChart, Bar, XAxis, YAxis, Tooltip, ResponsiveContainer,
  RadialBarChart, RadialBar, Legend
} from 'recharts'
import { api, fmt, fmtPct, PERIOD_STATUS } from '../api.js'

const AREA_COLORS = ['#34d399', '#22d3ee', '#a3e635', '#fbbf24', '#f87171', '#c084fc', '#fb923c', '#38bdf8']
const SCOPE_COLORS = ['#f87171', '#fbbf24', '#38bdf8']

export default function Dashboard({ notify }) {
  const [periods, setPeriods] = useState(null)
  const [periodId, setPeriodId] = useState('')
  const [summary, setSummary] = useState(null)
  const [busy, setBusy] = useState(false)

  useEffect(() => {
    api.getPeriods().then(p => {
      setPeriods(p)
      if (p.length) setPeriodId(p[0].Id)
    }).catch(e => notify(e.message, true))
  }, [])

  useEffect(() => {
    if (!periodId) return
    setSummary(null)
    api.getSummary(periodId).then(setSummary).catch(e => notify(e.message, true))
  }, [periodId])

  const recalc = async () => {
    setBusy(true)
    try {
      const s = await api.recalculate(periodId)
      setSummary(s)
      notify('محاسبه موجودی انتشار با موفقیت انجام شد')
    } catch (e) { notify(e.message, true) } finally { setBusy(false) }
  }

  const importData = async () => {
    setBusy(true)
    try {
      const n = await api.importActivityData(periodId)
      await api.importProductFootprints(periodId)
      setSummary(await api.getSummary(periodId))
      notify(`${n} ردیف داده فعالیت از منبع داده مرجع وارد شد`)
    } catch (e) { notify(e.message, true) } finally { setBusy(false) }
  }

  if (!periods) return <div className="spinner" />
  if (periods.length === 0) return (
    <div className="card"><div className="empty"><span className="ico">🗓️</span>هنوز دوره گزارش‌دهی تعریف نشده است.</div></div>
  )

  const period = periods.find(p => p.Id === periodId)
  const byScope = summary?.ByScope || []
  const byArea = (summary?.ByArea || []).map(a => ({ ...a, label: a.AreaFaName || a.AreaName }))
  const byCat = (summary?.ByCategory || []).filter(c => c.Co2e > 0).map(c => ({ ...c, label: c.CategoryFaName || c.CategoryName }))
  const products = summary?.Products || []
  const isEmpty = !summary || summary.TotalCo2e === 0

  return (
    <>
      <div className="toolbar">
        <select className="input" style={{ maxWidth: 340 }} value={periodId} onChange={e => setPeriodId(e.target.value)}>
          {periods.map(p => <option key={p.Id} value={p.Id}>{p.Title}</option>)}
        </select>
        {period && <span className={`chip ${period.Status === 3 ? 'green' : period.Status === 2 ? 'cyan' : 'yellow'}`}>
          {PERIOD_STATUS[period.Status]}
        </span>}
        <div style={{ flex: 1 }} />
        <button className="btn" onClick={importData} disabled={busy}>📥 ورود داده‌های مرجع</button>
        <button className="btn primary" onClick={recalc} disabled={busy}>{busy ? '⏳' : '🔄'} محاسبه موجودی</button>
      </div>

      {isEmpty ? (
        <div className="card"><div className="empty">
          <span className="ico">🧮</span>
          برای این دوره نتیجه‌ای محاسبه نشده است.<br />
          <span className="hint">ابتدا «ورود داده‌های مرجع» و سپس «محاسبه موجودی» را اجرا کنید.</span>
        </div></div>
      ) : (
        <>
          <div className="grid kpi" style={{ marginBottom: 18 }}>
            <div className="card kpi-card hoverable" style={{ '--kc': 'linear-gradient(135deg,#34d399,#22d3ee)' }}>
              <div className="kpi-ico">🌍</div>
              <div className="kpi-label">کل انتشار معادل CO₂</div>
              <div className="kpi-value">{fmt(summary.TotalCo2e)}</div>
              <div className="kpi-unit">tCO₂e در سال {period?.PersianYear}</div>
            </div>
            {byScope.map((s, i) => (
              <div className="card kpi-card hoverable" key={s.Scope} style={{ '--kc': `linear-gradient(135deg,${SCOPE_COLORS[i]},${SCOPE_COLORS[i]}88)` }}>
                <div className="kpi-ico">{['🏭', '⚡', '🚚'][s.Scope - 1]}</div>
                <div className="kpi-label">اسکوپ {s.Scope} ({['مستقیم', 'غیرمستقیم انرژی', 'سایر غیرمستقیم'][s.Scope - 1]})</div>
                <div className="kpi-value">{fmt(s.Co2e)}</div>
                <div className="kpi-unit">tCO₂e · سهم {fmtPct(s.SharePct)}</div>
              </div>
            ))}
          </div>

          <div className="grid two" style={{ marginBottom: 18 }}>
            <div className="card">
              <div className="card-title">🏭 انتشار به تفکیک ناحیه</div>
              <ResponsiveContainer width="100%" height={320}>
                <BarChart data={byArea} layout="vertical" margin={{ left: 10, right: 20 }}>
                  <XAxis type="number" tickFormatter={v => fmt(v, 0)} tick={{ fill: '#9fc4b3', fontSize: 11 }} />
                  <YAxis type="category" dataKey="label" width={95} tick={{ fill: '#e8f5ef', fontSize: 11.5 }} />
                  <Tooltip formatter={v => [`${fmt(v)} tCO₂e`, 'انتشار']} contentStyle={{ background: '#0a1b14', border: '1px solid rgba(74,222,168,.3)', borderRadius: 12, fontSize: 12 }} />
                  <Bar dataKey="Co2e" radius={[8, 0, 0, 8]}>
                    {byArea.map((_, i) => <Cell key={i} fill={AREA_COLORS[i % AREA_COLORS.length]} />)}
                  </Bar>
                </BarChart>
              </ResponsiveContainer>
            </div>
            <div className="card">
              <div className="card-title">🥧 سهم اسکوپ‌ها از کل انتشار</div>
              <ResponsiveContainer width="100%" height={320}>
                <PieChart>
                  <Pie data={byScope} dataKey="Co2e" nameKey="Scope" innerRadius={75} outerRadius={115} paddingAngle={4}>
                    {byScope.map((s, i) => <Cell key={i} fill={SCOPE_COLORS[i]} />)}
                  </Pie>
                  <Tooltip formatter={(v, n) => [`${fmt(v)} tCO₂e`, `اسکوپ ${n}`]} contentStyle={{ background: '#0a1b14', border: '1px solid rgba(74,222,168,.3)', borderRadius: 12, fontSize: 12 }} />
                  <Legend formatter={v => `اسکوپ ${v}`} wrapperStyle={{ fontSize: 12 }} />
                </PieChart>
              </ResponsiveContainer>
            </div>
          </div>

          <div className="grid two" style={{ marginBottom: 18 }}>
            <div className="card">
              <div className="card-title">🧩 انتشار به تفکیک دسته (۱۸ دسته ISO 14064-1)</div>
              <ResponsiveContainer width="100%" height={380}>
                <BarChart data={byCat} margin={{ bottom: 90 }}>
                  <XAxis dataKey="label" angle={-40} textAnchor="end" interval={0} tick={{ fill: '#9fc4b3', fontSize: 10 }} />
                  <YAxis tickFormatter={v => fmt(v, 0)} tick={{ fill: '#9fc4b3', fontSize: 11 }} />
                  <Tooltip formatter={v => [`${fmt(v)} tCO₂e`, 'انتشار']} contentStyle={{ background: '#0a1b14', border: '1px solid rgba(74,222,168,.3)', borderRadius: 12, fontSize: 12 }} />
                  <Bar dataKey="Co2e" radius={[8, 8, 0, 0]}>
                    {byCat.map((_, i) => <Cell key={i} fill={AREA_COLORS[i % AREA_COLORS.length]} />)}
                  </Bar>
                </BarChart>
              </ResponsiveContainer>
            </div>
            <div className="card">
              <div className="card-title">🏷️ جدول نواحی</div>
              <div className="table-wrap">
                <table className="tbl">
                  <thead><tr><th>ناحیه</th><th>انتشار (tCO₂e)</th><th>سهم</th><th></th></tr></thead>
                  <tbody>
                    {byArea.map((a, i) => (
                      <tr key={i}>
                        <td>{a.label}</td>
                        <td className="num">{fmt(a.Co2e)}</td>
                        <td className="num">{fmtPct(a.SharePct)}</td>
                        <td style={{ width: 90 }}>
                          <div className="bar-mini"><div className="track"><div className="fill" style={{ width: `${Math.min(a.SharePct, 100)}%` }} /></div></div>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </div>
          </div>

          {products.length > 0 && (
            <div className="card">
              <div className="card-title">📦 ردپای کربن محصولات (ISO 14067 — Cradle-to-Gate)</div>
              <ResponsiveContainer width="100%" height={300}>
                <BarChart data={products.map(p => ({ name: p.FaName || p.Name, cf: p.CarbonFootprint }))} margin={{ bottom: 20 }}>
                  <XAxis dataKey="name" tick={{ fill: '#e8f5ef', fontSize: 12 }} />
                  <YAxis tick={{ fill: '#9fc4b3', fontSize: 11 }} tickFormatter={v => v.toLocaleString('fa-IR')} />
                  <Tooltip formatter={v => [`${v} tCO₂e/t`, 'ردپای کربن']} contentStyle={{ background: '#0a1b14', border: '1px solid rgba(74,222,168,.3)', borderRadius: 12, fontSize: 12 }} />
                  <Bar dataKey="cf" radius={[8, 8, 0, 0]}>
                    {products.map((_, i) => <Cell key={i} fill={AREA_COLORS[i % AREA_COLORS.length]} />)}
                  </Bar>
                </BarChart>
              </ResponsiveContainer>
            </div>
          )}
        </>
      )}
    </>
  )
}
