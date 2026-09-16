import React, { useEffect, useState } from 'react'
import { api, fmt, CATEGORY_LABELS, STANDARD_LABELS } from '../api.js'

const EMPTY_FORMULA = {
  Code: '', Name: '', FaName: '', Category: 1, Expression: '',
  OutputUnit: 'tCO2e/y', Standard: 10, Reference: '', Notes: '', IsEnabled: true,
  Variables: []
}

function FormulaModal({ initial, onClose, onSave, notify }) {
  const [f, setF] = useState(initial)
  const [vars, setVars] = useState(JSON.stringify(initial.Variables || [], null, 2))
  const [testValues, setTestValues] = useState({})
  const [testResult, setTestResult] = useState(null)
  const set = (k, v) => setF(x => ({ ...x, [k]: v }))

  const runTest = async () => {
    try {
      const parsed = JSON.parse(vars || '[]')
      const body = { Expression: f.Expression, Values: testValues }
      if (initial.Id) { body.FormulaId = initial.Id } else { body.Code = f.Code }
      const res = await api.testFormula(body)
      setTestResult(res)
      if (!res.IsSuccess && parsed) onSave(null, parsed)
    } catch (e) {
      setTestResult({ IsSuccess: false, Error: e.message.includes('JSON') ? 'فرمت JSON متغیرها نامعتبر است' : e.message })
    }
  }

  const submit = async () => {
    let parsed
    try { parsed = JSON.parse(vars || '[]') } catch { notify('فرمت JSON متغیرها نامعتبر است', true); return }
    const body = { ...f, Variables: parsed }
    onSave(body)
  }

  return (
    <div className="modal-overlay" onClick={e => e.target === e.currentTarget && onClose()}>
      <div className="modal">
        <div className="modal-title">
          {initial.Id ? '✏️ ویرایش فرمول محاسبه' : '➕ فرمول محاسبه جدید'}
          <button className="btn sm ghost" onClick={onClose}>✕</button>
        </div>

        <div className="form-row" style={{ marginBottom: 12 }}>
          <div><label className="field">کد یکتا *</label>
            <input className="input" value={f.Code} onChange={e => set('Code', e.target.value)} dir="ltr" placeholder="GHG-COMBUSTION-GAS" /></div>
          <div><label className="field">دسته ضریب</label>
            <select className="input" value={f.Category ?? ''} onChange={e => set('Category', e.target.value ? Number(e.target.value) : null)}>
              <option value="">— بدون دسته —</option>
              {Object.entries(CATEGORY_LABELS).map(([k, v]) => <option key={k} value={k}>{v}</option>)}
            </select></div>
          <div><label className="field">استاندارد / منبع</label>
            <select className="input" value={f.Standard} onChange={e => set('Standard', Number(e.target.value))}>
              {Object.entries(STANDARD_LABELS).map(([k, v]) => <option key={k} value={k}>{v}</option>)}
            </select></div>
        </div>
        <div className="form-row" style={{ marginBottom: 12 }}>
          <div><label className="field">نام انگلیسی *</label>
            <input className="input" value={f.Name} onChange={e => set('Name', e.target.value)} dir="ltr" /></div>
          <div><label className="field">نام فارسی *</label>
            <input className="input" value={f.FaName} onChange={e => set('FaName', e.target.value)} /></div>
        </div>
        <div style={{ marginBottom: 12 }}>
          <label className="field">عبارت ریاضی * (متغیرها، اعداد، + - * / % ^ و توابع Min/Max/Abs/If/...)</label>
          <input className="input mono" value={f.Expression} onChange={e => set('Expression', e.target.value)} dir="ltr" placeholder="Consumption * LHV * EF" />
        </div>
        <div className="form-row" style={{ marginBottom: 12 }}>
          <div><label className="field">واحد خروجی</label>
            <input className="input" value={f.OutputUnit} onChange={e => set('OutputUnit', e.target.value)} dir="ltr" /></div>
          <div><label className="field">مرجع</label>
            <input className="input" value={f.Reference || ''} onChange={e => set('Reference', e.target.value)} /></div>
        </div>
        <div style={{ marginBottom: 12 }}>
          <label className="field">تعریف متغیرها (JSON) — مبنای تولید پویا و خودکار فرم ورود داده</label>
          <textarea className="input mono" rows={7} value={vars} onChange={e => setVars(e.target.value)} dir="ltr" style={{ fontSize: 11.5 }} />
        </div>

        <div className="card" style={{ padding: 14, marginBottom: 4 }}>
          <div className="card-title" style={{ marginBottom: 10 }}>🧪 آزمایش فرمول</div>
          <div className="form-row" style={{ marginBottom: 10 }}>
            <div>
              <label className="field">مقادیر آزمایشی (name = value)</label>
              <input className="input mono" dir="ltr" placeholder='مثال: Consumption=100, LHV=15'
                onChange={e => {
                  const obj = {}
                  e.target.value.split(',').filter(s => s.includes('=')).forEach(s => {
                    const [k, v] = s.split('=')
                    if (k?.trim() && !isNaN(Number(v))) obj[k.trim()] = Number(v)
                  })
                  setTestValues(obj)
                }} />
            </div>
          </div>
          <button className="btn sm" onClick={runTest}>▶ اجرای آزمایش</button>
          {testResult && (
            <div style={{ marginTop: 10 }}>
              {testResult.IsSuccess
                ? <span className="chip green">✅ نتیجه: {fmt(testResult.Result, 6)}</span>
                : <span className="chip red">⛔ {testResult.Error}</span>}
            </div>
          )}
        </div>

        <div className="modal-actions">
          <button className="btn primary" onClick={submit}>💾 ذخیره</button>
          <button className="btn ghost" onClick={onClose}>انصراف</button>
        </div>
      </div>
    </div>
  )
}

function FactorModal({ initial, onClose, onSave }) {
  const [f, setF] = useState(initial)
  const set = (k, v) => setF(x => ({ ...x, [k]: v }))
  return (
    <div className="modal-overlay" onClick={e => e.target === e.currentTarget && onClose()}>
      <div className="modal" style={{ width: 'min(560px,100%)' }}>
        <div className="modal-title">{initial.Id ? '✏️ ویرایش ضریب انتشار' : '➕ ضریب انتشار جدید'}
          <button className="btn sm ghost" onClick={onClose}>✕</button></div>
        <div className="form-row" style={{ marginBottom: 12 }}>
          <div><label className="field">دسته *</label>
            <select className="input" value={f.Category} onChange={e => set('Category', Number(e.target.value))}>
              {Object.entries(CATEGORY_LABELS).map(([k, v]) => <option key={k} value={k}>{v}</option>)}
            </select></div>
          <div><label className="field">کلید مرجع *</label>
            <input className="input" value={f.RefKey} onChange={e => set('RefKey', e.target.value)} dir="ltr" /></div>
          <div><label className="field">گاز</label>
            <input className="input" value={f.Gas} onChange={e => set('Gas', e.target.value)} dir="ltr" /></div>
        </div>
        <div className="form-row" style={{ marginBottom: 12 }}>
          <div><label className="field">زیرکلید</label>
            <input className="input" value={f.SubKey || ''} onChange={e => set('SubKey', e.target.value)} dir="ltr" /></div>
          <div><label className="field">مقدار *</label>
            <input className="input" type="number" step="any" value={f.Value} onChange={e => set('Value', Number(e.target.value))} dir="ltr" /></div>
          <div><label className="field">واحد *</label>
            <input className="input" value={f.Unit} onChange={e => set('Unit', e.target.value)} dir="ltr" /></div>
        </div>
        <div className="form-row" style={{ marginBottom: 12 }}>
          <div><label className="field">استاندارد</label>
            <select className="input" value={f.Standard} onChange={e => set('Standard', Number(e.target.value))}>
              {Object.entries(STANDARD_LABELS).map(([k, v]) => <option key={k} value={k}>{v}</option>)}
            </select></div>
          <div><label className="field">سال اعتبار</label>
            <input className="input" type="number" value={f.ValidFromYear} onChange={e => set('ValidFromYear', Number(e.target.value))} dir="ltr" /></div>
        </div>
        <div style={{ marginBottom: 12 }}>
          <label className="field">برچسب فارسی</label>
          <input className="input" value={f.RefLabel || ''} onChange={e => set('RefLabel', e.target.value)} />
        </div>
        <div className="modal-actions">
          <button className="btn primary" onClick={() => onSave(f)}>💾 ذخیره</button>
          <button className="btn ghost" onClick={onClose}>انصراف</button>
        </div>
      </div>
    </div>
  )
}

export default function Settings({ notify }) {
  const [tab, setTab] = useState('formulas')
  const [formulas, setFormulas] = useState(null)
  const [factors, setFactors] = useState(null)
  const [params, setParams] = useState(null)
  const [gwps, setGwps] = useState(null)
  const [fuels, setFuels] = useState(null)
  const [modal, setModal] = useState(null)
  const [factorCat, setFactorCat] = useState('')

  useEffect(() => { api.getFormulas().then(setFormulas).catch(e => notify(e.message, true)) }, [])
  useEffect(() => { api.getFactors(factorCat ? { category: factorCat } : {}).then(setFactors).catch(e => notify(e.message, true)) }, [factorCat])
  useEffect(() => { api.getParameters().then(setParams).catch(() => {}) }, [])
  useEffect(() => { api.getGwps().then(setGwps).catch(() => {}) }, [])
  useEffect(() => { api.getFuels().then(setFuels).catch(() => {}) }, [])

  const saveFormula = async (body) => {
    try {
      if (body === null) return
      if (modal.id) { await api.updateFormula({ ...body, Id: modal.id }) } else { await api.createFormula(body) }
      notify('فرمول ذخیره شد')
      setModal(null)
      api.getFormulas().then(setFormulas)
    } catch (e) { notify(e.message, true) }
  }

  const saveFactor = async (body) => {
    try {
      if (modal.id) await api.updateFactor(body)
      else await api.createFactor(body)
      notify('ضریب ذخیره شد')
      setModal(null)
      api.getFactors(factorCat ? { category: factorCat } : {}).then(setFactors)
    } catch (e) { notify(e.message, true) }
  }

  const saveParam = async (p) => {
    try {
      if (p.Id) await api.updateParameter(p)
      else await api.createParameter(p)
      notify('پارامتر ذخیره شد')
      api.getParameters().then(setParams)
    } catch (e) { notify(e.message, true) }
  }

  const delFormula = async (id) => {
    try { await api.deleteFormula(id); api.getFormulas().then(setFormulas); notify('فرمول حذف شد') }
    catch (e) { notify(e.message, true) }
  }

  return (
    <>
      <div className="tabs">
        {[['formulas', '🧮 فرمول‌های محاسبه'], ['factors', '📊 ضرایب انتشار'], ['params', '⚙️ پارامترها'], ['gwp', '🌡️ GWP گازها'], ['fuels', '⛽ سوخت‌ها و LHV']].map(([k, l]) => (
          <div key={k} className={`tab ${tab === k ? 'active' : ''}`} onClick={() => setTab(k)}>{l}</div>
        ))}
      </div>

      {tab === 'formulas' && (
        <div className="card">
          <div className="card-title">
            فرمول‌های محاسبه (قابل ویرایش — پایه CBAM / LCA در آینده)
            <button className="btn primary sm" style={{ marginRight: 'auto' }}
              onClick={() => setModal({ type: 'formula', data: EMPTY_FORMULA })}>➕ فرمول جدید</button>
          </div>
          {!formulas ? <div className="spinner" /> : !formulas.Items?.length ? <div className="empty"><span className="ico">🧮</span>فرمولی ثبت نشده است.</div> : (
            <div className="table-wrap">
              <table className="tbl">
                <thead><tr><th>کد</th><th>عنوان</th><th>عبارت</th><th>خروجی</th><th>استاندارد</th><th></th></tr></thead>
                <tbody>
                  {formulas.Items.map(f => (
                    <tr key={f.Id}>
                      <td className="mono" style={{ fontSize: 11 }}>{f.Code}</td>
                      <td>{f.FaName}<div className="hint">{f.Name}</div></td>
                      <td><code className="mono" style={{ fontSize: 11, color: 'var(--acc-3)' }}>{f.Expression}</code></td>
                      <td className="mono" style={{ fontSize: 11 }}>{f.OutputUnit}</td>
                      <td><span className="chip">{STANDARD_LABELS[f.Standard]}</span></td>
                      <td><div className="actions">
                        <button className="btn sm" onClick={() => setModal({ type: 'formula', data: f, id: f.Id })}>ویرایش</button>
                        <button className="btn sm danger" onClick={() => delFormula(f.Id)}>حذف</button>
                      </div></td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      )}

      {tab === 'factors' && (
        <div className="card">
          <div className="card-title">
            ضرایب انتشار — استخراج شده از شیت Settings ورک‌بوک MSC-GHG
            <button className="btn primary sm" style={{ marginRight: 'auto' }}
              onClick={() => setModal({ type: 'factor', data: { Category: 1, RefKey: '', RefLabel: '', SubKey: '', Gas: 'CO2e', Value: 0, Unit: 'tCO2e/t', Standard: 1, ValidFromYear: 1403 } })}>➕ ضریب جدید</button>
          </div>
          <div className="toolbar">
            <select className="input" value={factorCat} onChange={e => setFactorCat(e.target.value)}>
              <option value="">همه دسته‌ها</option>
              {Object.entries(CATEGORY_LABELS).map(([k, v]) => <option key={k} value={k}>{v}</option>)}
            </select>
          </div>
          {!factors ? <div className="spinner" /> : !factors.Items?.length ? <div className="empty"><span className="ico">📊</span>ضریبی یافت نشد.</div> : (
            <div className="table-wrap">
              <table className="tbl">
                <thead><tr><th>دسته</th><th>مرجع</th><th>گاز</th><th>مقدار</th><th>واحد</th><th>منبع</th><th></th></tr></thead>
                <tbody>
                  {factors.Items.map(f => (
                    <tr key={f.Id}>
                      <td><span className="chip cyan" style={{ fontSize: 10 }}>{CATEGORY_LABELS[f.Category]}</span></td>
                      <td>{f.RefLabel || f.RefKey}{f.SubKey ? <span className="hint"> ({f.SubKey})</span> : ''}</td>
                      <td className="mono">{f.Gas}</td>
                      <td className="num">{f.Value.toPrecision(6).replace(/e([+-])(\d+)/i, '×10^$1$2')}</td>
                      <td className="mono" style={{ fontSize: 11 }}>{f.Unit}</td>
                      <td><span className="chip" style={{ fontSize: 10 }}>{STANDARD_LABELS[f.Standard]}</span></td>
                      <td><div className="actions">
                        <button className="btn sm" onClick={() => setModal({ type: 'factor', data: f, id: f.Id })}>ویرایش</button>
                      </div></td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      )}

      {tab === 'params' && (
        <div className="card">
          <div className="card-title">پارامترهای عمومی محاسبات (شیت Settings — جدول Parameters)</div>
          {!params ? <div className="spinner" /> : (
            <div className="table-wrap">
              <table className="tbl">
                <thead><tr><th>کلید</th><th>عنوان</th><th>مقدار</th><th>واحد</th><th>سال</th><th></th></tr></thead>
                <tbody>
                  {params.Items?.map(p => (
                    <tr key={p.Id}>
                      <td className="mono" style={{ fontSize: 11 }}>{p.Key}</td>
                      <td>{p.FaName || p.Name}</td>
                      <td className="num" style={{ color: 'var(--acc)' }}>
                        <input className="input" style={{ width: 120, padding: '5px 10px', direction: 'ltr' }} type="number" step="any"
                          defaultValue={p.Value} onBlur={e => { if (Number(e.target.value) !== p.Value) saveParam({ ...p, Value: Number(e.target.value) }) }} />
                      </td>
                      <td className="mono" style={{ fontSize: 11 }}>{p.Unit || '-'}</td>
                      <td className="num">{p.Year}</td>
                      <td className="src-note">{p.Source}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
          <div className="hint" style={{ marginTop: 10 }}>برای تغییر مقدار، عدد را ویرایش کنید — با خروج از فیلد ذخیره می‌شود.</div>
        </div>
      )}

      {tab === 'gwp' && (
        <div className="card">
          <div className="card-title">پتانسیل گرمایش جهانی (IPCC AR6 — Settings!B23)</div>
          {!gwps ? <div className="spinner" /> : (
            <div className="grid" style={{ gridTemplateColumns: 'repeat(auto-fit,minmax(180px,1fr))' }}>
              {gwps.map(g => (
                <div key={g.Id} className="card hoverable" style={{ padding: 16, textAlign: 'center' }}>
                  <div className="kpi-label mono">{g.GasKey}</div>
                  <div className="kpi-value" style={{ fontSize: 20 }}>{g.Value.toLocaleString('fa-IR')}</div>
                  <div className="kpi-unit">{g.AssessmentReport} · افق {g.TimeHorizon} سال</div>
                </div>
              ))}
            </div>
          )}
        </div>
      )}

      {tab === 'fuels' && (
        <div className="card">
          <div className="card-title">سوخت‌ها و ارزش حرارتی پایین (LHV — Settings!B29)</div>
          {!fuels ? <div className="spinner" /> : (
            <div className="table-wrap">
              <table className="tbl">
                <thead><tr><th>سوخت</th><th>LHV</th><th>واحد LHV</th><th>واحد مصرف</th><th>منبع</th></tr></thead>
                <tbody>
                  {fuels.map(f => (
                    <tr key={f.Id}>
                      <td>{f.FaName || f.Name}</td>
                      <td className="num" style={{ color: 'var(--acc)' }}>{f.Lhv}</td>
                      <td className="mono" style={{ fontSize: 11 }}>{f.LhvUnit}</td>
                      <td className="mono" style={{ fontSize: 11 }}>{f.ConsumptionUnit}</td>
                      <td className="src-note">{f.Source}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      )}

      {modal?.type === 'formula' && (
        <FormulaModal initial={modal.data} onClose={() => setModal(null)} onSave={saveFormula} notify={notify} />
      )}
      {modal?.type === 'factor' && (
        <FactorModal initial={modal.data} onClose={() => setModal(null)} onSave={saveFactor} />
      )}
    </>
  )
}
