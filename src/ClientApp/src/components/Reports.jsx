import React, { useEffect, useState } from 'react'
import { RadialBarChart, RadialBar, PolarAngleAxis, ResponsiveContainer } from 'recharts'
import { api, fmt, fmtPct, PERIOD_STATUS } from '../api.js'

export default function Reports({ notify }) {
  const [periods, setPeriods] = useState([])
  const [periodId, setPeriodId] = useState('')
  const [summary, setSummary] = useState(null)

  useEffect(() => {
    api.getPeriods().then(p => { setPeriods(p); if (p.length) setPeriodId(p[0].Id) }).catch(e => notify(e.message, true))
  }, [])
  useEffect(() => {
    if (periodId) api.getSummary(periodId).then(setSummary).catch(e => notify(e.message, true))
  }, [periodId])

  if (!periods.length) return <div className="card"><div className="empty"><span className="ico">📑</span>دوره‌ای تعریف نشده است.</div></div>

  const period = periods.find(p => p.Id === periodId)
  const products = summary?.Products || []
  const byCat = (summary?.ByCategory || []).slice().sort((a, b) => b.Co2e - a.Co2e)

  return (
    <>
      <div className="toolbar">
        <select className="input" style={{ maxWidth: 340 }} value={periodId} onChange={e => setPeriodId(e.target.value)}>
          {periods.map(p => <option key={p.Id} value={p.Id}>{p.Title}</option>)}
        </select>
        {period && <span className={`chip ${period.Status === 3 ? 'green' : period.Status === 2 ? 'cyan' : 'yellow'}`}>{PERIOD_STATUS[period.Status]}</span>}
      </div>

      {!summary ? <div className="spinner" /> : (
        <>
          {/* گزارش 1: موجودی انتشار - معادل شیت Inventory و Tables & Graphs */}
          <div className="card" style={{ marginBottom: 18 }}>
            <div className="card-title">📑 گزارش موجودی انتشار گازهای گلخانه‌ای (ISO 14064-1:2018)</div>
            <div className="kpi-label" style={{ marginBottom: 14 }}>
              کل انتشار سال {period?.PersianYear}: <b style={{ color: 'var(--acc)', fontSize: 15 }}>{fmt(summary.TotalCo2e)} tCO₂e</b>
            </div>
            <div className="table-wrap">
              <table className="tbl">
                <thead>
                  <tr><th>ردیف</th><th>دسته انتشار</th><th>علامت</th><th>کد ISO</th><th>اسکوپ</th><th>انتشار (tCO₂e)</th><th>سهم</th></tr>
                </thead>
                <tbody>
                  {byCat.map((c, i) => (
                    <tr key={i}>
                      <td className="num">{i + 1}</td>
                      <td>{c.CategoryFaName || c.CategoryName}</td>
                      <td className="mono">{c.Sign}</td>
                      <td className="num">{c.CategoryNo}</td>
                      <td><span className={`chip ${c.Scope === 1 ? 'red' : c.Scope === 2 ? 'yellow' : 'cyan'}`} style={{ fontSize: 10 }}>اسکوپ {c.Scope}</span></td>
                      <td className="num" style={{ color: 'var(--acc)' }}>{fmt(c.Co2e)}</td>
                      <td className="num">{fmtPct(c.SharePct)}</td>
                    </tr>
                  ))}
                </tbody>
                <tfoot>
                  <tr style={{ fontWeight: 800, borderTop: '2px solid var(--stroke-2)' }}>
                    <td colSpan={5}>جمع کل</td>
                    <td className="num" style={{ color: 'var(--acc)' }}>{fmt(summary.TotalCo2e)}</td>
                    <td className="num">۱۰۰٪</td>
                  </tr>
                </tfoot>
              </table>
            </div>
          </div>

          {/* گزارش 2: ردپای کربن محصولات - معادل شیت Units CF */}
          {products.length > 0 && (
            <div className="card" style={{ marginBottom: 18 }}>
              <div className="card-title">📦 گزارش ردپای کربن محصولات (ISO 14067:2018 — Cradle-to-Gate)</div>
              <div className="grid" style={{ gridTemplateColumns: 'repeat(auto-fit,minmax(260px,1fr))' }}>
                {products.map((p, i) => (
                  <div key={i} className="card hoverable" style={{ padding: 18 }}>
                    <ResponsiveContainer width="100%" height={120}>
                      <RadialBarChart innerRadius="68%" outerRadius="100%" data={[{ v: Math.min(p.CarbonFootprint / 3, 1) * 100 }]} startAngle={210} endAngle={-30}>
                        <PolarAngleAxis type="number" domain={[0, 100]} tick={false} />
                        <RadialBar dataKey="v" cornerRadius={12} fill={['#34d399', '#22d3ee', '#a3e635', '#fbbf24', '#f87171'][i % 5]} background={{ fill: 'rgba(6,20,14,.8)' }} />
                      </RadialBarChart>
                    </ResponsiveContainer>
                    <div style={{ textAlign: 'center', marginTop: -88, marginBottom: 46 }}>
                      <div style={{ fontSize: 22, fontWeight: 800, color: 'var(--acc)' }}>{p.CarbonFootprint}</div>
                      <div style={{ fontSize: 10, color: 'var(--txt-faint)' }}>tCO₂e / ton</div>
                    </div>
                    <div style={{ fontWeight: 700, fontSize: 14 }}>{p.FaName || p.Name}</div>
                    <div className="hint">ناحیه: {p.AreaName} · تولید سالانه: {fmt(p.AnnualProduction)} تن</div>
                    <div style={{ marginTop: 8 }}>
                      <span className="chip green" style={{ fontSize: 10 }}>{p.Boundary}</span>{' '}
                      <span className="chip" style={{ fontSize: 10 }}>{p.Standard}</span>
                    </div>
                  </div>
                ))}
              </div>
            </div>
          )}

          {/* گزارش 3: خلاصه اجرایی */}
          <div className="card">
            <div className="card-title">📌 خلاصه اجرایی — روش‌شناسی</div>
            <div style={{ fontSize: 13, lineHeight: 2.1, color: 'var(--txt-dim)' }}>
              • مرز محاسبه: <b style={{ color: 'var(--txt)' }}>همه نواحی مجتمع (۸ ناحیه)</b> مطابق ISO 14064-1:2018 — بخش ۱ (موجودی انتشار)<br />
              • ردپای کربن محصولات مطابق <b style={{ color: 'var(--txt)' }}>ISO 14067:2018</b> — بخش ۲، مرز Cradle-to-Gate<br />
              • GWP: گزارش ششم IPCC (AR6) · CH₄=27، N₂O=273، SF₆=24300<br />
              • فرمول پایه احتراق: <span className="mono" style={{ color: 'var(--acc-3)' }}>E = Consumption × LHV × EF</span> ؛ معادل‌سازی: <span className="mono" style={{ color: 'var(--acc-3)' }}>CO₂e = CO₂ + CH₄×GWP + N₂O×GWP</span><br />
              • منابع ضرایب: IPCC 2006، DEFRA-UK 2024، GHG Protocol V2.7، ECOINVENT V3.11، WSA 2024، ترازنامه انرژی ۱۴۰۱<br />
              • داده اولیه: ورک‌بوک GHG Atlas MSC نسخه ۲.۲ + بسته فایل‌های ۱۴۰۳ (پسماند، سرباره، آنالیز گاز) — آماده جایگزینی با سرویس وب<br />
              • دستورالعمل حاکم: EMSPR مدیریت ردپای کربن (۱۴۰۴/۰۳/۲۰)
            </div>
          </div>
        </>
      )}
    </>
  )
}
