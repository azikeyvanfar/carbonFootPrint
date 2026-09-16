// ---------- پیکربندی و لایه دسترسی به API ----------
const API_BASE = import.meta.env.VITE_API_BASE || 'http://localhost:44231'

export const TOKEN_KEY = 'cf_access_token'

export function getToken() {
  return localStorage.getItem(TOKEN_KEY)
}
export function setToken(token) {
  localStorage.setItem(TOKEN_KEY, token)
}
export function clearToken() {
  localStorage.removeItem(TOKEN_KEY)
}

function pascalCaseKey(key) {
  return key ? key.charAt(0).toUpperCase() + key.slice(1) : key
}

function normalizeResponse(value) {
  if (Array.isArray(value)) return value.map(normalizeResponse)
  if (!value || typeof value !== 'object') return value
  return Object.fromEntries(
    Object.entries(value).map(([key, item]) => [pascalCaseKey(key), normalizeResponse(item)])
  )
}

async function request(path, { method = 'GET', body, headers = {} } = {}) {
  const token = getToken()
  const res = await fetch(`${API_BASE}${path}`, {
    method,
    headers: {
      'Content-Type': 'application/json',
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...headers
    },
    body: body ? JSON.stringify(body) : undefined
  })
  if (res.status === 401) {
    if (token) {
      clearToken()
      window.location.reload()
      throw new Error('نشست منقضی شده است')
    }
    let authError = null
    try { authError = await res.json() } catch { /* no body */ }
    throw new Error(authError?.title || 'نام کاربری یا کلمه عبور نادرست است')
  }
  let payload = null
  try { payload = normalizeResponse(await res.json()) } catch { /* no body */ }
  if (!res.ok) {
    const msgs = payload?.Message?.length ? payload.Message.join(' و ') : (payload?.title || payload?.detail || `خطای ${res.status}`)
    throw new Error(typeof msgs === 'string' ? msgs : JSON.stringify(msgs))
  }
  return payload
}

// ---------- Account ----------
export const api = {
  login: (username, password) =>
    request('/api/Core/Account/LoginDevelop', { method: 'POST', body: { username, password } }).then(r => r.Data),

  // ---------- Settings: فرمول‌ها ----------
  getFormulas: (page = 1, pageSize = 100) =>
    request(`/api/Ghg/Formulas/GetAll?page=${page}&pageSize=${pageSize}`).then(r => r.Data),
  getFormula: (id) =>
    request(`/api/Ghg/Formulas/GetById?id=${id}`).then(r => r.Data),
  createFormula: (body) =>
    request('/api/Ghg/Formulas/Create', { method: 'POST', body }).then(r => r.Data),
  updateFormula: (body) =>
    request('/api/Ghg/Formulas/Update', { method: 'POST', body }).then(r => r.Data),
  deleteFormula: (id) =>
    request('/api/Ghg/Formulas/Delete', { method: 'POST', body: { id } }).then(r => r.Data),
  testFormula: (body) =>
    request('/api/Ghg/Formulas/Test', { method: 'POST', body }).then(r => r.Data),

  // ---------- Settings: ضرایب ----------
  getFactors: (params = {}) => {
    const q = new URLSearchParams({ page: params.page || 1, pageSize: params.pageSize || 200 })
    if (params.category != null) q.set('category', params.category)
    if (params.search) q.set('search', params.search)
    return request(`/api/Ghg/Factors/GetAll?${q}`).then(r => r.Data)
  },
  createFactor: (body) =>
    request('/api/Ghg/Factors/Create', { method: 'POST', body }).then(r => r.Data),
  updateFactor: (body) =>
    request('/api/Ghg/Factors/Update', { method: 'POST', body }).then(r => r.Data),
  deleteFactor: (id) =>
    request('/api/Ghg/Factors/Delete', { method: 'POST', body: { id } }).then(r => r.Data),

  // ---------- Settings: داده‌های مرجع ----------
  getAreas: () => request('/api/Ghg/Settings/GetAreas').then(r => r.Data),
  getCostCenters: (page = 1, pageSize = 300, areaId) => {
    let q = `page=${page}&pageSize=${pageSize}`
    if (areaId) q += `&areaId=${areaId}`
    return request(`/api/Ghg/Settings/GetCostCenters?${q}`).then(r => r.Data)
  },
  getCategories: () => request('/api/Ghg/Settings/GetCategories').then(r => r.Data),
  getFuels: () => request('/api/Ghg/Settings/GetFuels').then(r => r.Data),
  getGwps: () => request('/api/Ghg/Settings/GetGwps').then(r => r.Data),
  getParameters: (page = 1, pageSize = 100) =>
    request(`/api/Ghg/Settings/GetParameters?page=${page}&pageSize=${pageSize}`).then(r => r.Data),
  createParameter: (body) =>
    request('/api/Ghg/Settings/CreateParameter', { method: 'POST', body }).then(r => r.Data),
  updateParameter: (body) =>
    request('/api/Ghg/Settings/UpdateParameter', { method: 'POST', body }).then(r => r.Data),

  // ---------- Periods ----------
  getPeriods: () => request('/api/Ghg/Periods/GetAll').then(r => r.Data),
  createPeriod: (body) =>
    request('/api/Ghg/Periods/Create', { method: 'POST', body }).then(r => r.Data),
  getSummary: (periodId) =>
    request(`/api/Ghg/Periods/GetSummary?periodId=${periodId}`).then(r => r.Data),
  recalculate: (periodId) =>
    request('/api/Ghg/Periods/Recalculate', { method: 'POST', body: { periodId } }).then(r => r.Data),
  importActivityData: (periodId) =>
    request('/api/Ghg/Periods/ImportActivityData', { method: 'POST', body: { periodId } }).then(r => r.Data),
  importProductFootprints: (periodId) =>
    request('/api/Ghg/Periods/ImportProductFootprints', { method: 'POST', body: { periodId } }).then(r => r.Data),

  // ---------- Activity Data / Wizard ----------
  getActivityData: (periodId, page = 1, pageSize = 50, categoryId) => {
    let q = `periodId=${periodId}&page=${page}&pageSize=${pageSize}`
    if (categoryId) q += `&categoryId=${categoryId}`
    return request(`/api/Ghg/ActivityData/GetAll?${q}`).then(r => r.Data)
  },
  getWizard: (periodId) =>
    request(`/api/Ghg/ActivityData/GetWizard?periodId=${periodId}`).then(r => r.Data),
  createActivity: (body) =>
    request('/api/Ghg/ActivityData/Create', { method: 'POST', body }).then(r => r.Data),
  updateActivity: (body) =>
    request('/api/Ghg/ActivityData/Update', { method: 'POST', body }).then(r => r.Data),
  deleteActivity: (id) =>
    request('/api/Ghg/ActivityData/Delete', { method: 'POST', body: { id } }).then(r => r.Data),
  previewActivity: (body) =>
    request('/api/Ghg/ActivityData/Preview', { method: 'POST', body }).then(r => r.Data)
}

// ---------- Labels ----------
export const CATEGORY_LABELS = {
  1: 'سوخت - احتراق ثابت', 2: 'تولید برق', 3: 'حمل و نقل مواد', 4: 'حمل و نقل داخل سایت',
  5: 'رفت و آمد پرسنل', 6: 'سفرهای کاری', 7: 'مدیریت پسماند', 8: 'مواد خریداری شده',
  9: 'مرتبط با انرژی', 10: 'تجهیزات فرار', 11: 'سیلاب‌ها', 12: 'فاضلاب'
}

export const STANDARD_LABELS = {
  0: 'محاسبه در این مطالعه', 1: 'IPCC 2006', 2: 'IPCC AR6', 3: 'DEFRA-UK 2024',
  4: 'GHG Protocol', 5: 'ECOINVENT', 6: 'World Steel', 7: 'IEA 2023', 8: 'TCEQ',
  9: 'API 2009', 10: 'ISO 14064-1', 11: 'ISO 14067', 12: 'CBAM', 13: 'LCA', 99: 'سایر'
}

export const PERIOD_STATUS = { 1: 'پیش‌نویس', 2: 'محاسبه شده', 3: 'منتشر شده' }

export const DATA_SOURCES = {
  1: 'سفارش خرید', 2: 'قبض', 3: 'اندازه‌گیری', 4: 'نقشه P&ID', 5: 'برآورد',
  6: 'محاسبه', 7: 'گزارش حمل و نقل', 8: 'مصاحبه پرسنل', 9: 'سرویس وب'
}

export function fmt(n, digits = 2) {
  if (n == null || isNaN(n)) return '-'
  if (Math.abs(n) >= 1e9) return (n / 1e9).toLocaleString('fa-IR', { maximumFractionDigits: 2 }) + ' میلیارد'
  if (Math.abs(n) >= 1e6) return (n / 1e6).toLocaleString('fa-IR', { maximumFractionDigits: 2 }) + ' میلیون'
  return n.toLocaleString('fa-IR', { maximumFractionDigits: digits })
}
export function fmtPct(n) {
  if (n == null || isNaN(n)) return '-'
  return n.toLocaleString('fa-IR', { maximumFractionDigits: 1 }) + '٪'
}
