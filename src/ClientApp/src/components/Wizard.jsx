import React, { useEffect, useMemo, useState } from 'react'
import { api, fmt, DATA_SOURCES } from '../api.js'

// فیلدهای عددی هر علامت دسته (منطبق با منطق ورک‌بوک)
const EXTRA_FIELDS = {
  C: [{ name: 'FuelId', label: 'نوع سوخت', type: 'fuel', required: true },
      { name: 'Quantity', label: 'مصرف سالانه', type: 'number', required: true }],
  V: [{ name: 'FactorRefKey', label: 'فرآیند (Lime Making / EAFs / Direct Reduction 1...)', type: 'text', required: true },
      { name: 'Quantity', label: 'تولید سالانه (ton)', type: 'number' },
      { name: 'Quantity2', label: 'خوراک گاز (kNm3)', type: 'number' }],
  F: [{ name: 'Quantity', label: 'تعداد شیرها', type: 'number', required: true },
      { name: 'Quantity2', label: 'تعداد فلنج‌ها', type: 'number' },
      { name: 'Quantity3', label: 'تعداد PSV ها', type: 'number' },
      { name: 'ControlEfficiency', label: 'راندمان کنترل (٪ 0-100)', type: 'number' }],
  E: [{ name: 'FactorRefKey', label: 'نوع برق (Total Average / Direct Contract / Solar / MSC Production)', type: 'text', required: true },
      { name: 'Quantity', label: 'مصرف برق (kWh/y)', type: 'number', required: true }],
  W: [{ name: 'FactorRefKey', label: 'نوع پسماند (Metal / Non-Metal / Plastic)', type: 'text', required: true },
      { name: 'FactorSubKey', label: 'روش مدیریت (Sale / Landfill)', type: 'text', required: true },
      { name: 'Quantity', label: 'مقدار (ton)', type: 'number', required: true }],
  Z: [{ name: 'FactorRefKey', label: 'نوع پسماند (Metal / Non-Metal / Plastic)', type: 'text', required: true },
      { name: 'FactorSubKey', label: 'روش مدیریت (Sale / Landfill)', type: 'text', required: true },
      { name: 'Quantity', label: 'مقدار (ton)', type: 'number', required: true }],
  Y: [{ name: 'Quantity', label: 'دبی فاضلاب (m3/y)', type: 'number', required: true }],
  M: [{ name: 'FactorRefKey', label: 'سوخت (Petrol / Gas oil / CNG)', type: 'text', required: true },
      { name: 'Quantity', label: 'مصرف (Lit)', type: 'number', required: true }],
  T: [{ name: 'FactorRefKey', label: 'نوع حمل (Road / Rail / Marine / Air)', type: 'text', required: true },
      { name: 'Quantity', label: 'وزن بار (ton)', type: 'number', required: true },
      { name: 'Quantity2', label: 'مسافت (km)', type: 'number', required: true }],
  D: [{ name: 'FactorRefKey', label: 'نوع حمل (Road / Rail / Marine / Air)', type: 'text', required: true },
      { name: 'Quantity', label: 'وزن بار (ton)', type: 'number', required: true },
      { name: 'Quantity2', label: 'مسافت (km)', type: 'number', required: true }],
  P: [{ name: 'FactorRefKey', label: 'وسیله (Car / Mini Bus / Bus)', type: 'text', required: true },
      { name: 'Quantity', label: 'مسافت (km)', type: 'number', required: true },
      { name: 'Quantity2', label: 'تعداد افراد', type: 'number' }],
  B: [{ name: 'FactorRefKey', label: 'وسیله (Car / Bus / Air - Short Haul / Train...)', type: 'text', required: true },
      { name: 'Quantity', label: 'مسافت (km)', type: 'number', required: true },
      { name: 'Quantity2', label: 'تعداد سفر', type: 'number' }],
  R: [{ name: 'FactorRefKey', label: 'نام ماده (DRI / Lime / Scrap / Pellet...)', type: 'text', required: true },
      { name: 'Quantity', label: 'مقدار (ton)', type: 'number', required: true }],
  L: [{ name: 'FactorRefKey', label: 'نام ماده مصرفی', type: 'text', required: true },
      { name: 'Quantity', label: 'مقدار (ton)', type: 'number', required: true }],
  X: [{ name: 'FactorRefKey', label: 'نام خدمت', type: 'text', required: true },
      { name: 'Quantity', label: 'مقدار', type: 'number', required: true }]
}

const PREVIEW_CODES = {
  C: 'GHG-COMBUSTION-GAS', E: 'GHG-ELECTRICITY', F: 'GHG-FUGITIVE-EQUIPMENT',
  W: 'GHG-WASTE', Z: 'GHG-WASTE', T: 'GHG-TRANSPORT-MATERIAL', D: 'GHG-TRANSPORT-MATERIAL',
  P: 'GHG-COMMUTING', B: 'GHG-COMMUTING', R: 'GHG-PURCHASED-MATERIAL',
  L: 'GHG-PURCHASED-MATERIAL', X: 'GHG-PURCHASED-MATERIAL', Y: 'GHG-WASTEWATER-CH4', V: null, M: null
}

export default function Wizard({ notify }) {
  const [periods, setPeriods] = useState([])
  const [periodId, setPeriodId] = useState('')
  const [wizard, setWizard] = useState(null)
  const [areas, setAreas] = useState([])
  const [fuels, setFuels] = useState([])
  const [costCenters, setCostCenters] = useState([])
  const [activeStep, setActiveStep] = useState(0)
  const [form, setForm] = useState({})
  const [preview, setPreview] = useState(null)
  const [rows, setRows] = useState(null)
  const [busy, setBusy] = useState(false)

  useEffect(() => {
    Promise.all([api.getPeriods(), api.getAreas(), api.getFuels()])
      .then(([ps, as, fs]) => {
        setPeriods(ps); setAreas(as); setFuels(fs)
        if (ps.length) setPeriodId(ps[0].Id)
      }).catch(e => notify(e.message, true))
  }, [])

  useEffect(() => {
    if (!periodId) return
    api.getWizard(periodId).then(setWizard).catch(e => notify(e.message, true))
  }, [periodId])

  useEffect(() => { if (areas.length) api.getCostCenters().then(r => setCostCenters(r.Items || [])).catch(() => {}) }, [areas.length])

  const loadRows = async (categoryId) => {
    try {
      const r = await api.getActivityData(periodId, 1, 50, categoryId)
      setRows(r)
    } catch (e) { notify(e.message, true) }
  }

  useEffect(() => { if (periodId) loadRows(undefined) }, [periodId])

  const step = wizard?.Steps?.[activeStep]
  const stepFields = useMemo(() => {
    if (!step) return []

    // Prefer the API metadata generated from configured workbook formulas.
    // Keep the local map as a compatibility fallback for older deployments.
    const serverFields = (step.Fields || [])
      .filter(f => !['AreaId', 'CostCenterId', 'EmissionSource'].includes(f.Name))
      .map(f => ({
        name: f.Name,
        label: f.Label,
        type: f.Type || 'number',
        unit: f.Unit,
        required: f.IsRequired,
        helpText: f.HelpText
      }))

    return serverFields.length
      ? serverFields
      : (EXTRA_FIELDS[step.Sign] || [{ name: 'Quantity', label: 'مقدار', type: 'number', required: true }])
  }, [step])

  const setField = (k, v) => setForm(f => ({ ...f, [k]: v }))

  const doPreview = async () => {
    setPreview(null)
    const code = step && PREVIEW_CODES[step.Sign]
    if (!code) { notify('برای این دسته پیش‌نمایش مستقیم فرمول در دسترس نیست') ; return }
    const values = {}
    Object.entries(form).forEach(([k, v]) => {
      if (typeof v === 'number' && !isNaN(v)) values[k] = v
    })
    if (form.ControlEfficiency != null) values.ControlEfficiency = form.ControlEfficiency / 100
    try {
      const res = await api.previewActivity({ code, values })
      setPreview(res)
    } catch (e) { notify(e.message, true) }
  }

  const save = async () => {
    if (!step) return
    if (!form.AreaId) { notify('انتخاب ناحیه الزامی است', true); return }
    const required = stepFields.filter(f => f.required)
    for (const f of required) {
      if (form[f.name] == null || form[f.name] === '') { notify(`تکمیل «${f.label}» الزامی است`, true); return }
    }
    setBusy(true)
    try {
      await api.createActivity({
        periodId,
        categoryId: step.CategoryId,
        areaId: form.AreaId || null,
        costCenterId: form.CostCenterId || null,
        fuelId: form.FuelId || null,
        factorRefKey: form.FactorRefKey || null,
        factorSubKey: form.FactorSubKey || null,
        emissionSource: form.EmissionSource || null,
        quantity: Number(form.Quantity) || 0,
        quantity2: form.Quantity2 != null && form.Quantity2 !== '' ? Number(form.Quantity2) : null,
        quantity3: form.Quantity3 != null && form.Quantity3 !== '' ? Number(form.Quantity3) : null,
        unit: null,
        controlEfficiency: form.ControlEfficiency != null && form.ControlEfficiency !== '' ? Number(form.ControlEfficiency) / 100 : null,
        dataSource: Number(form.DataSource) || 3,
        description: form.Description || null
      })
      notify('ردیف داده فعالیت ذخیره شد')
      setForm({})
      setPreview(null)
      loadRows(step.CategoryId)
    } catch (e) { notify(e.message, true) } finally { setBusy(false) }
  }

  const delRow = async (id) => {
    try { await api.deleteActivity(id); loadRows(step?.CategoryId) ; notify('ردیف حذف شد') }
    catch (e) { notify(e.message, true) }
  }

  if (!periods.length) return <div className="card"><div className="empty"><span className="ico">🗓️</span>دوره‌ای تعریف نشده است.</div></div>
  if (!wizard) return <div className="spinner" />

  return (
    <>
      <div className="toolbar">
        <select className="input" style={{ maxWidth: 340 }} value={periodId} onChange={e => setPeriodId(e.target.value)}>
          {periods.map(p => <option key={p.Id} value={p.Id}>{p.Title}</option>)}
        </select>
      </div>

      <div className="wizard-steps">
        {wizard.Steps.map((s, i) => (
          <div key={s.CategoryId} className={`wstep ${i === activeStep ? 'active' : ''}`} onClick={() => { setActiveStep(i); setPreview(null) }}>
            <span className="n">{i + 1}</span>
            <span>{s.FaName}<span style={{ opacity: .5, fontSize: 10 }}> ({s.Sign} · اسکوپ {s.Scope})</span></span>
          </div>
        ))}
      </div>

      <div className="wizard-body">
        <div className="card">
          <div className="card-title">
            گام {activeStep + 1} از {wizard.Steps.length} — {step?.FaName}
            <span className="chip cyan" style={{ marginRight: 'auto' }}>دسته {step?.CategoryNo} ISO 14064-1 · اسکوپ {step?.Scope}</span>
          </div>

          <div className="form-row" style={{ marginBottom: 14 }}>
            <div>
              <label className="field">ناحیه *</label>
              <select className="input" value={form.AreaId || ''} onChange={e => setField('AreaId', e.target.value || null)}>
                <option value="">انتخاب کنید...</option>
                {areas.map(a => <option key={a.Id} value={a.Id}>{a.FaName} ({a.Code})</option>)}
              </select>
            </div>
            <div>
              <label className="field">مرکز هزینه</label>
              <select className="input" value={form.CostCenterId || ''} onChange={e => setField('CostCenterId', e.target.value || null)}>
                <option value="">انتخاب کنید...</option>
                {costCenters.filter(c => !form.AreaId || c.AreaId === form.AreaId).map(c => (
                  <option key={c.Id} value={c.Id}>{c.Code} — {c.Name}</option>
                ))}
              </select>
            </div>
            <div>
              <label className="field">منبع داده</label>
              <select className="input" value={form.DataSource || 3} onChange={e => setField('DataSource', e.target.value)}>
                {Object.entries(DATA_SOURCES).map(([k, v]) => <option key={k} value={k}>{v}</option>)}
              </select>
            </div>
          </div>

          <div className="form-row" style={{ marginBottom: 14 }}>
            {stepFields.map(f => (
              <div key={f.name}>
                <label className="field">
                  {f.label} {f.required ? '*' : ''}{f.unit ? ` (${f.unit})` : ''}
                </label>
                {f.type === 'fuel' ? (
                  <select className="input" value={form.FuelId || ''} onChange={e => setField('FuelId', e.target.value || null)}>
                    <option value="">انتخاب کنید...</option>
                    {fuels.map(x => <option key={x.Id} value={x.Id}>{x.FaName || x.Name} ({x.ConsumptionUnit})</option>)}
                  </select>
                ) : (
                  <input className="input" type={f.type === 'number' ? 'number' : 'text'} step="any"
                    value={form[f.name] ?? ''} title={f.helpText || ''}
                    onChange={e => setField(f.name, f.type === 'number' ? (e.target.value === '' ? '' : Number(e.target.value)) : e.target.value)} />
                )}
                {f.helpText && <div className="hint">{f.helpText}</div>}
              </div>
            ))}
            <div style={{ gridColumn: '1 / -1' }}>
              <label className="field">شرح منبع انتشار</label>
              <input className="input" value={form.EmissionSource || ''} onChange={e => setField('EmissionSource', e.target.value)} placeholder="مثال: احتراق گاز طبیعی کوره‌های نورد" />
            </div>
          </div>

          {step?.Description && (
            <div className="formula-box" style={{ marginBottom: 14 }}>
              {step.Description.split('\n').filter(Boolean).map((l, i) => <div key={i}>{l}</div>)}
            </div>
          )}

          <div className="modal-actions">
            <button className="btn primary" onClick={save} disabled={busy}>💾 ذخیره ردیف</button>
            <button className="btn" onClick={doPreview} disabled={busy}>👁️ پیش‌نمایش محاسبه</button>
            <button className="btn ghost" onClick={() => { setForm({}); setPreview(null) }}>پاک کردن فرم</button>
          </div>
        </div>

        <div>
          {preview && (
            <div className="card" style={{ marginBottom: 18 }}>
              <div className="card-title">⚡ پیش‌نمایش نتیجه فرمول</div>
              {preview.IsSuccess ? (
                <div className="preview-result">
                  <div className="val">{fmt(preview.Result, 4)}</div>
                  <div className="unit">{preview.OutputUnit}</div>
                  <div className="formula-box" style={{ marginTop: 12, fontSize: 11 }}>{preview.Expression}</div>
                </div>
              ) : (
                <div className="empty" style={{ color: 'var(--danger)' }}>⛔ {preview.Error}</div>
              )}
            </div>
          )}
          <div className="card">
            <div className="card-title">📋 ردیف‌های ثبت‌شده {step ? `— ${step.FaName}` : ''}</div>
            {!rows || !rows.Items?.length ? (
              <div className="empty"><span className="ico">📭</span>ردیفی ثبت نشده است.</div>
            ) : (
              <div className="table-wrap">
                <table className="tbl" style={{ minWidth: 480 }}>
                  <thead><tr><th>ناحیه</th><th>مرکز هزینه</th><th>مرجع ضریب</th><th>کمیت</th><th></th></tr></thead>
                  <tbody>
                    {rows.Items.map(r => (
                      <tr key={r.Id}>
                        <td>{r.AreaName || '-'}</td>
                        <td>{r.CostCenterName || '-'}</td>
                        <td>{r.FactorRefKey || r.FuelName || '-'}</td>
                        <td className="num">{fmt(r.Quantity)}</td>
                        <td><button className="btn sm danger" onClick={() => delRow(r.Id)}>حذف</button></td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            )}
          </div>
        </div>
      </div>
    </>
  )
}
