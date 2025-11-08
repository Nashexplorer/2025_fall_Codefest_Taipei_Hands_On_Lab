# 2025 Fall Codefest Taipei - Hands On Lab 專案說明文件

## 目錄
- [專案基本資訊](#專案基本資訊)
- [技術架構](#技術架構)
- [功能模組](#功能模組)
- [專案結構](#專案結構)
- [開始使用](#開始使用)
- [開發指南](#開發指南)
- [部署](#部署)
- [貢獻指南](#貢獻指南)
- [授權與聯絡資訊](#授權與聯絡資訊)

---

## 專案基本資訊

### 專案名稱
**2025 Fall Codefest Taipei Hands On Lab**

### 專案簡介
本專案是為 2025 年秋季台北程式碼競賽（Codefest Taipei）所建立的測試環境與實作練習平台。採用現代化的 Web 開發技術棧，提供參賽者一個完整的開發環境範本，用於快速建立前端應用程式。

**核心功能：**
- 提供現代化的 React 前端開發環境
- 整合最新版本的 Next.js 框架與 Turbopack 打包工具
- 支援 TypeScript 強型別開發
- 內建 Tailwind CSS 快速樣式開發
- 提供即時熱更新（Hot Reload）的開發體驗

### 主要解決的問題
1. **開發環境標準化**：為參賽者提供統一的開發環境配置，減少環境設定時間
2. **技術棧現代化**：整合業界最新的前端開發技術，提升開發效率
3. **快速原型開發**：透過 Next.js App Router 架構，快速實現功能原型
4. **型別安全保障**：使用 TypeScript 提供編譯時期的型別檢查，降低執行時期錯誤
5. **樣式開發效率**：整合 Tailwind CSS，提供 utility-first 的樣式開發方式

### 目標使用者
- 參與 2025 Fall Codefest Taipei 的開發者與團隊
- 學習現代化前端開發技術的開發者
- 需要快速建立 Next.js 專案的開發團隊
- 對 React 19 與 Next.js 15 感興趣的技術愛好者

---

## 技術架構

### 使用的程式語言與版本
- **TypeScript**：^5.x
  - 提供靜態型別檢查
  - 支援最新的 ES 標準特性
  - 編譯目標為 ES2017
- **JavaScript (ES2017+)**
  - 支援現代 JavaScript 語法
  - 完整的 ESNext 模組系統

### 主要框架與函式庫

#### 核心框架
- **Next.js**：15.5.6
  - 採用 App Router 架構
  - 支援 Server Components 與 Client Components
  - 內建影像最佳化（Image Optimization）
  - 整合 Turbopack 提升建構速度

#### UI 函式庫
- **React**：19.1.0
  - 最新版本的 React，支援並行渲染特性
  - 改善的 Hooks 效能
- **React DOM**：19.1.0
  - React 的 DOM 渲染引擎

#### 樣式框架
- **Tailwind CSS**：^4.x
  - Utility-first CSS 框架
  - 支援深色模式（Dark Mode）
  - 響應式設計（RWD）
- **PostCSS**：^4.x
  - CSS 預處理器
  - 配合 Tailwind CSS 使用

#### 字型系統
- **Geist Font Family**
  - 來自 Vercel 的現代字型
  - 包含 Sans 與 Mono 兩種字型
  - 透過 `next/font/google` 自動最佳化載入

#### 開發工具
- **ESLint**：^9.x
  - 程式碼風格檢查與規範
  - 整合 Next.js 推薦配置
- **TypeScript 型別定義**
  - @types/node：^20.x
  - @types/react：^19.x
  - @types/react-dom：^19.x

### 資料庫類型
目前專案為前端應用範本，**尚未整合資料庫**。

建議的資料庫選項：
- **PostgreSQL**：適合關聯式資料儲存
- **MongoDB**：適合文件導向資料儲存
- **Supabase**：開源的 Firebase 替代方案
- **Vercel KV/Postgres**：與 Vercel 平台深度整合

### 系統架構模式
本專案採用 **Next.js App Router 架構**，具備以下特點：

#### 1. 檔案系統路由（File-based Routing）
- 基於檔案結構自動產生路由
- 支援動態路由、巢狀路由、平行路由等進階功能

#### 2. Server-First 架構
- 預設使用 React Server Components
- 優化首次載入效能與 SEO
- 減少客戶端 JavaScript 體積

#### 3. 模組化元件架構
- 元件導向開發
- 可重用的 UI 元件
- 清晰的關注點分離（Separation of Concerns）

#### 4. 漸進式增強（Progressive Enhancement）
- 核心功能不依賴 JavaScript
- 支援靜態生成（SSG）與伺服器端渲染（SSR）

### 第三方服務或 API
當前專案為基礎範本，可輕鬆整合以下服務：

- **Vercel 部署平台**：專案原生支援部署至 Vercel
- **Next.js 內建 API Routes**：可快速建立 RESTful API 端點
- **環境變數管理**：支援 `.env.local` 進行環境配置

---

## 功能模組

### 主要功能模組

#### 1. 頁面路由模組（App Router）
**職責：**
- 管理應用程式的路由結構
- 處理頁面導航與切換
- 支援動態路由參數

**相關檔案：**
- `src/app/page.tsx`：首頁元件
- `src/app/layout.tsx`：根版面配置

#### 2. 版面配置模組（Layout）
**職責：**
- 定義全域版面結構
- 管理共用的 UI 元素（如導航列、側邊欄）
- 設定全域樣式與字型
- 配置網頁 metadata（標題、描述等）

**核心功能：**
- 整合 Geist 字型系統
- 提供一致的頁面架構
- 支援巢狀版面配置

#### 3. 樣式系統模組
**職責：**
- 管理全域樣式
- 提供 Tailwind CSS 工具類別
- 支援深色模式切換
- 響應式設計實作

**相關檔案：**
- `src/app/globals.css`：全域 CSS 樣式
- `postcss.config.mjs`：PostCSS 配置
- `tailwind.config.js`：Tailwind 設定（隱含）

#### 4. 影像最佳化模組
**職責：**
- 自動優化圖片載入
- 提供響應式圖片
- 支援延遲載入（Lazy Loading）
- 自動格式轉換（WebP、AVIF）

**使用方式：**
```typescript
import Image from "next/image";
```

#### 5. 靜態資源管理模組
**職責：**
- 管理專案的靜態資源（圖片、圖示等）
- 提供公開訪問路徑

**資源目錄：**
- `public/`：存放靜態檔案（SVG、圖片等）

### 模組間的關聯關係

```
┌─────────────────────────────────────┐
│      App Router (page.tsx)          │
│         主頁面元件                   │
└──────────────┬──────────────────────┘
               │
               ↓
┌─────────────────────────────────────┐
│    Root Layout (layout.tsx)         │
│       根版面配置                     │
│  • 字型系統                          │
│  • 全域樣式                          │
│  • Metadata                         │
└──────────────┬──────────────────────┘
               │
               ↓
┌─────────────────────────────────────┐
│      樣式系統 (globals.css)         │
│    • Tailwind CSS                   │
│    • 客製化樣式                      │
└─────────────────────────────────────┘
               │
               ↓
┌─────────────────────────────────────┐
│      靜態資源 (public/)             │
│    • SVG 圖示                       │
│    • 品牌資源                        │
└─────────────────────────────────────┘
```

**資料流向：**
1. 使用者訪問應用程式
2. Next.js 路由器解析 URL
3. 載入對應的頁面元件（page.tsx）
4. 套用根版面配置（layout.tsx）
5. 載入全域樣式（globals.css）
6. 渲染完整頁面

---

## 專案結構

### 目錄結構說明

```
2025_fall_Codefest_Taipei_Hands_On_Lab/
│
├── frontend/                          # 前端應用程式根目錄
│   └── my-next-app/                  # Next.js 應用程式
│       ├── src/                      # 原始碼目錄
│       │   └── app/                  # App Router 目錄（Next.js 15+）
│       │       ├── page.tsx          # 首頁元件
│       │       ├── layout.tsx        # 根版面配置
│       │       ├── globals.css       # 全域 CSS 樣式
│       │       └── favicon.ico       # 網站圖示
│       │
│       ├── public/                   # 靜態資源目錄（公開訪問）
│       │   ├── next.svg             # Next.js Logo
│       │   ├── vercel.svg           # Vercel Logo
│       │   ├── file.svg             # 檔案圖示
│       │   ├── globe.svg            # 地球圖示
│       │   └── window.svg           # 視窗圖示
│       │
│       ├── package.json              # NPM 套件相依性定義
│       ├── package-lock.json         # NPM 套件版本鎖定
│       ├── tsconfig.json            # TypeScript 編譯器設定
│       ├── next.config.ts           # Next.js 框架配置
│       ├── postcss.config.mjs       # PostCSS 設定
│       ├── eslint.config.mjs        # ESLint 程式碼規範設定
│       └── README.md                # Next.js 專案說明
│
├── LICENSE                           # MIT 開源授權條款
└── README.md                         # 專案根目錄說明文件
```

### 重要檔案與資料夾的用途

#### 核心配置檔案

| 檔案名稱 | 用途說明 |
|---------|---------|
| `package.json` | 定義專案相依套件、腳本指令、專案元資訊 |
| `tsconfig.json` | TypeScript 編譯器配置，包含路徑別名、編譯選項 |
| `next.config.ts` | Next.js 框架的進階配置（重新導向、環境變數等） |
| `eslint.config.mjs` | ESLint 程式碼風格檢查規則 |
| `postcss.config.mjs` | PostCSS 與 Tailwind CSS 整合配置 |

#### 原始碼目錄

**`src/app/`** - App Router 核心目錄
- `page.tsx`：定義路由頁面的主要內容
- `layout.tsx`：定義版面配置與共用結構
- `globals.css`：全域樣式表，包含 Tailwind 指令
- `favicon.ico`：瀏覽器標籤頁圖示

**`public/`** - 靜態資源目錄
- 此目錄內的檔案可透過根路徑直接訪問
- 例如：`public/logo.png` → `http://localhost:3000/logo.png`
- 適合存放圖片、字型、robots.txt、sitemap.xml 等

#### 自動生成的檔案（不應手動修改）

- `package-lock.json`：NPM 套件依賴樹的完整快照
- `.next/`：Next.js 建構產出目錄（未納入版本控制）
- `node_modules/`：NPM 套件安裝目錄（未納入版本控制）

---

## 開始使用

### 環境需求

在開始之前，請確保您的開發環境符合以下需求：

#### 必要軟體
- **Node.js**：18.18.0 或以上版本（建議使用 LTS 版本）
  - 檢查版本：`node --version`
  - 下載位置：[https://nodejs.org/](https://nodejs.org/)

- **npm**：9.0.0 或以上版本（通常隨 Node.js 一同安裝）
  - 檢查版本：`npm --version`

#### 可選套件管理工具
- **yarn**：1.22.0 或以上
- **pnpm**：8.0.0 或以上
- **bun**：最新版本

#### 作業系統
- Windows 10/11
- macOS 10.15 或以上
- Linux（任何現代發行版）

#### 開發工具建議
- **Visual Studio Code**（推薦）
  - 安裝擴充套件：ESLint、Tailwind CSS IntelliSense、TypeScript
- **WebStorm**
- **其他支援 TypeScript 的編輯器**

### 安裝步驟

#### 1. 複製專案

```bash
# 使用 Git 複製專案
git clone https://github.com/your-username/2025_fall_Codefest_Taipei_Hands_On_Lab.git

# 進入專案目錄
cd 2025_fall_Codefest_Taipei_Hands_On_Lab
```

#### 2. 安裝相依套件

```bash
# 進入前端專案目錄
cd frontend/my-next-app

# 使用 npm 安裝（推薦）
npm install

# 或使用 yarn
yarn install

# 或使用 pnpm
pnpm install

# 或使用 bun
bun install
```

安裝過程會：
- 下載所有相依套件到 `node_modules/`
- 生成 `package-lock.json`（如果不存在）
- 執行 postinstall 腳本（如有定義）

#### 3. 啟動開發伺服器

```bash
# 使用 npm
npm run dev

# 或使用 yarn
yarn dev

# 或使用 pnpm
pnpm dev

# 或使用 bun
bun dev
```

#### 4. 開啟瀏覽器

開發伺服器啟動後，開啟瀏覽器並訪問：
- **本機位址**：[http://localhost:3000](http://localhost:3000)
- **網路位址**：[http://[您的IP]:3000](http://[您的IP]:3000)

您應該會看到 Next.js 的歡迎頁面。

### 設定說明

#### 環境變數配置

Next.js 支援透過 `.env` 檔案管理環境變數。

**建立環境變數檔案：**

```bash
# 在 frontend/my-next-app/ 目錄下建立
touch .env.local
```

**環境變數檔案類型：**
- `.env.local`：本機開發環境（**不應納入版本控制**）
- `.env.development`：開發環境
- `.env.production`：正式環境
- `.env`：所有環境的預設值

**範例設定：**

```bash
# .env.local

# API 端點
NEXT_PUBLIC_API_URL=https://api.example.com

# 應用程式設定
NEXT_PUBLIC_APP_NAME=Codefest Taipei 2025

# 僅在伺服器端可用的變數（不加 NEXT_PUBLIC_ 前綴）
DATABASE_URL=postgresql://localhost:5432/mydb
API_SECRET_KEY=your-secret-key-here
```

**重要事項：**
- `NEXT_PUBLIC_` 前綴的變數會暴露到瀏覽器端
- 沒有此前綴的變數僅在伺服器端可用
- 修改環境變數後需重新啟動開發伺服器

#### TypeScript 路徑別名

專案已預設配置路徑別名，可簡化 import 路徑：

```typescript
// 使用別名前
import Button from '../../../components/Button';

// 使用別名後
import Button from '@/components/Button';
```

**配置位置：** `tsconfig.json`

```json
{
  "compilerOptions": {
    "paths": {
      "@/*": ["./src/*"]
    }
  }
}
```

#### Tailwind CSS 客製化

您可以在專案根目錄新增 `tailwind.config.js` 來客製化 Tailwind：

```javascript
/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    './src/**/*.{js,ts,jsx,tsx,mdx}',
  ],
  theme: {
    extend: {
      colors: {
        primary: '#your-color',
      },
    },
  },
  plugins: [],
}
```

### 執行方式

#### 開發模式（Development Mode）

```bash
npm run dev
```

**特點：**
- 啟用 Turbopack 快速熱更新
- 詳細的錯誤訊息與警告
- 不進行程式碼最佳化（保持可讀性）
- 自動重新載入頁面

#### 正式環境建構（Production Build）

```bash
# 建構專案
npm run build

# 啟動正式環境伺服器
npm run start
```

**建構產出：**
- 最佳化的 JavaScript Bundle
- 靜態檔案生成（如適用）
- 壓縮的 CSS
- 影像最佳化

#### 程式碼檢查（Linting）

```bash
npm run lint
```

**檢查項目：**
- TypeScript 型別錯誤
- ESLint 程式碼風格
- Next.js 最佳實踐建議

---

## API 文件

> **注意：** 本專案目前為前端應用範本，尚未實作後端 API。以下為建議的 API 架構說明。

### 使用 Next.js API Routes 建立 API

Next.js 提供內建的 API Routes 功能，您可以在 `src/app/api/` 目錄下建立 API 端點。

#### 建立 API 端點範例

**檔案路徑：** `src/app/api/hello/route.ts`

```typescript
import { NextResponse } from 'next/server';

// GET /api/hello
export async function GET() {
  return NextResponse.json({
    message: 'Hello from Codefest Taipei 2025!',
    timestamp: new Date().toISOString(),
  });
}

// POST /api/hello
export async function POST(request: Request) {
  const body = await request.json();
  
  return NextResponse.json({
    received: body,
    status: 'success',
  });
}
```

#### API 端點列表（範例）

| 方法 | 端點 | 說明 | 認證 |
|-----|------|-----|------|
| `GET` | `/api/health` | 健康檢查端點 | 否 |
| `GET` | `/api/hello` | 測試端點，返回歡迎訊息 | 否 |
| `POST` | `/api/hello` | 接收並回傳 JSON 資料 | 否 |

#### 請求/回應格式範例

**GET /api/hello**

*請求：*
```bash
curl http://localhost:3000/api/hello
```

*回應：*
```json
{
  "message": "Hello from Codefest Taipei 2025!",
  "timestamp": "2025-11-01T12:00:00.000Z"
}
```

**POST /api/hello**

*請求：*
```bash
curl -X POST http://localhost:3000/api/hello \
  -H "Content-Type: application/json" \
  -d '{"name": "Developer", "team": "Team A"}'
```

*回應：*
```json
{
  "received": {
    "name": "Developer",
    "team": "Team A"
  },
  "status": "success"
}
```

#### 錯誤處理

建議使用統一的錯誤回應格式：

```typescript
// 錯誤回應範例
{
  "error": {
    "code": "INVALID_REQUEST",
    "message": "請求格式不正確",
    "details": {
      "field": "email",
      "issue": "必須是有效的電子郵件地址"
    }
  },
  "timestamp": "2025-11-01T12:00:00.000Z"
}
```

**HTTP 狀態碼：**
- `200` - 成功
- `201` - 建立成功
- `400` - 請求錯誤
- `401` - 未授權
- `404` - 找不到資源
- `500` - 伺服器錯誤

---

## 開發指南

### 如何建立開發環境

#### 1. 安裝開發工具

**Visual Studio Code 擴充套件（推薦）：**

```bash
# 透過指令安裝
code --install-extension dbaeumer.vscode-eslint
code --install-extension bradlc.vscode-tailwindcss
code --install-extension esbenp.prettier-vscode
```

或在 VS Code 擴充套件市集搜尋安裝：
- **ESLint** - 程式碼檢查
- **Tailwind CSS IntelliSense** - Tailwind 自動完成
- **Prettier** - 程式碼格式化
- **TypeScript Importer** - 自動 import

**VS Code 設定檔（.vscode/settings.json）：**

```json
{
  "editor.formatOnSave": true,
  "editor.defaultFormatter": "esbenp.prettier-vscode",
  "editor.codeActionsOnSave": {
    "source.fixAll.eslint": true
  },
  "typescript.tsdk": "node_modules/typescript/lib",
  "tailwindCSS.experimental.classRegex": [
    ["cva\\(([^)]*)\\)", "[\"'`]([^\"'`]*).*?[\"'`]"]
  ]
}
```

#### 2. Git Hooks 設定（可選）

使用 Husky 設定 Git Hooks，確保提交前程式碼品質：

```bash
# 安裝 Husky
npm install --save-dev husky

# 初始化 Husky
npx husky init

# 新增 pre-commit hook
echo "npm run lint" > .husky/pre-commit
```

#### 3. 開發資料庫設定（如需要）

如果您的專案需要資料庫，建議使用 Docker：

```yaml
# docker-compose.yml
version: '3.8'
services:
  postgres:
    image: postgres:16
    environment:
      POSTGRES_USER: devuser
      POSTGRES_PASSWORD: devpass
      POSTGRES_DB: codefest_db
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data

volumes:
  postgres_data:
```

啟動：
```bash
docker-compose up -d
```

### 程式碼撰寫規範

#### TypeScript 規範

1. **優先使用型別推斷**
```typescript
// ✅ 好的寫法
const message = "Hello";

// ❌ 不必要的型別宣告
const message: string = "Hello";
```

2. **為函式參數與回傳值標註型別**
```typescript
// ✅ 好的寫法
function calculateTotal(items: CartItem[]): number {
  return items.reduce((sum, item) => sum + item.price, 0);
}

// ❌ 避免使用 any
function process(data: any) {
  // ...
}
```

3. **使用介面（Interface）定義物件結構**
```typescript
interface User {
  id: string;
  name: string;
  email: string;
  role: 'admin' | 'user';
}
```

#### React 元件規範

1. **使用函式元件與 Hooks**
```typescript
// ✅ 好的寫法 - 函式元件
export function UserProfile({ userId }: { userId: string }) {
  const [user, setUser] = useState<User | null>(null);
  
  return <div>{user?.name}</div>;
}

// ❌ 避免 - Class 元件（除非必要）
export class UserProfile extends React.Component {
  // ...
}
```

2. **元件檔案命名**
- 使用 PascalCase：`UserProfile.tsx`
- 一個檔案一個主要元件
- 工具函式使用 camelCase：`formatDate.ts`

3. **Props 型別定義**
```typescript
interface ButtonProps {
  label: string;
  onClick: () => void;
  variant?: 'primary' | 'secondary';
  disabled?: boolean;
}

export function Button({ 
  label, 
  onClick, 
  variant = 'primary',
  disabled = false 
}: ButtonProps) {
  // ...
}
```

#### 樣式規範

1. **Tailwind CSS 使用原則**
```tsx
// ✅ 好的寫法 - 使用 Tailwind 工具類別
<button className="rounded-lg bg-blue-500 px-4 py-2 text-white hover:bg-blue-600">
  Click Me
</button>

// ❌ 避免 - 內聯樣式
<button style={{ backgroundColor: 'blue', padding: '8px 16px' }}>
  Click Me
</button>
```

2. **複雜樣式抽取為變數**
```typescript
const buttonStyles = {
  base: "rounded-lg px-4 py-2 font-medium transition-colors",
  primary: "bg-blue-500 text-white hover:bg-blue-600",
  secondary: "bg-gray-200 text-gray-800 hover:bg-gray-300",
};
```

#### 資料夾結構規範

```
src/
├── app/                      # App Router 頁面
│   ├── (auth)/              # 路由群組：認證相關頁面
│   ├── api/                 # API Routes
│   └── dashboard/           # Dashboard 頁面
│
├── components/              # 可重用元件
│   ├── ui/                 # 基礎 UI 元件（Button、Input）
│   ├── features/           # 功能特定元件
│   └── layout/             # 版面配置元件
│
├── lib/                    # 工具函式與配置
│   ├── utils.ts           # 通用工具函式
│   ├── api.ts             # API 客戶端
│   └── constants.ts       # 常數定義
│
├── types/                  # TypeScript 型別定義
│   └── index.ts
│
└── hooks/                  # 自訂 React Hooks
    └── useAuth.ts
```

### 分支策略

本專案採用 **Git Flow** 分支模型：

#### 主要分支

1. **`main`** - 正式環境分支
   - 永遠保持可部署狀態
   - 只接受來自 `develop` 或 `hotfix` 的 merge

2. **`develop`** - 開發整合分支
   - 最新的開發進度
   - 功能分支的整合點

#### 支援分支

3. **`feature/*`** - 功能開發分支
```bash
# 建立功能分支
git checkout -develop
git checkout -b feature/user-authentication

# 完成後合併回 develop
git checkout develop
git merge --no-ff feature/user-authentication
git branch -d feature/user-authentication
```

4. **`bugfix/*`** - 錯誤修復分支
```bash
git checkout -b bugfix/fix-login-error develop
```

5. **`hotfix/*`** - 緊急修復分支
```bash
# 從 main 分支建立
git checkout -b hotfix/security-patch main

# 完成後同時合併回 main 和 develop
git checkout main
git merge --no-ff hotfix/security-patch
git checkout develop
git merge --no-ff hotfix/security-patch
```

#### Commit 訊息規範

採用 **Conventional Commits** 格式：

```
<type>(<scope>): <subject>

<body>

<footer>
```

**Type 類型：**
- `feat`: 新功能
- `fix`: 錯誤修復
- `docs`: 文件更新
- `style`: 程式碼格式調整（不影響功能）
- `refactor`: 重構
- `perf`: 效能改善
- `test`: 測試相關
- `chore`: 建構流程或輔助工具變動

**範例：**
```bash
git commit -m "feat(auth): add user login functionality"
git commit -m "fix(ui): resolve button alignment issue on mobile"
git commit -m "docs: update API documentation"
```

### 如何執行測試

> **注意：** 本專案範本目前尚未設定測試框架。以下為建議的測試設定。

#### 安裝測試套件

```bash
# 安裝 Jest 與 React Testing Library
npm install --save-dev jest @testing-library/react @testing-library/jest-dom
npm install --save-dev @testing-library/user-event
npm install --save-dev jest-environment-jsdom
```

#### Jest 配置

建立 `jest.config.js`：

```javascript
const nextJest = require('next/jest');

const createJestConfig = nextJest({
  dir: './',
});

const customJestConfig = {
  setupFilesAfterEnv: ['<rootDir>/jest.setup.js'],
  testEnvironment: 'jest-environment-jsdom',
  moduleNameMapper: {
    '^@/(.*)$': '<rootDir>/src/$1',
  },
};

module.exports = createJestConfig(customJestConfig);
```

#### 執行測試

```bash
# 執行所有測試
npm test

# 監視模式
npm test -- --watch

# 產生覆蓋率報告
npm test -- --coverage
```

#### 測試範例

```typescript
// components/Button.test.tsx
import { render, screen, fireEvent } from '@testing-library/react';
import { Button } from './Button';

describe('Button Component', () => {
  it('renders with correct label', () => {
    render(<Button label="Click Me" onClick={() => {}} />);
    expect(screen.getByText('Click Me')).toBeInTheDocument();
  });

  it('calls onClick when clicked', () => {
    const handleClick = jest.fn();
    render(<Button label="Click" onClick={handleClick} />);
    
    fireEvent.click(screen.getByText('Click'));
    expect(handleClick).toHaveBeenCalledTimes(1);
  });
});
```

---

## 部署

### 部署流程

#### 方案一：部署至 Vercel（推薦）

Vercel 是 Next.js 的官方部署平台，提供最佳整合體驗。

**步驟：**

1. **連結 Git 儲存庫**
   - 前往 [vercel.com](https://vercel.com)
   - 使用 GitHub/GitLab/Bitbucket 登入
   - 點擊「New Project」
   - 選擇您的專案儲存庫

2. **配置專案設定**
   ```
   Framework Preset: Next.js
   Root Directory: frontend/my-next-app
   Build Command: npm run build
   Output Directory: .next
   Install Command: npm install
   ```

3. **設定環境變數**
   - 在 Vercel 專案設定中新增環境變數
   - 區分 Production、Preview、Development 環境

4. **部署**
   - 點擊「Deploy」
   - Vercel 會自動建構並部署專案
   - 每次 push 到 main 分支會自動觸發部署

**自動部署設定：**
- Main 分支 → Production 環境
- Develop 分支 → Preview 環境
- Pull Request → 臨時預覽環境

#### 方案二：部署至其他平台

**Netlify：**
```bash
# 安裝 Netlify CLI
npm install -g netlify-cli

# 建構專案
npm run build

# 部署
netlify deploy --prod
```

**Docker 部署：**

建立 `Dockerfile`：
```dockerfile
FROM node:20-alpine AS builder

WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run build

FROM node:20-alpine AS runner
WORKDIR /app

ENV NODE_ENV=production

COPY --from=builder /app/public ./public
COPY --from=builder /app/.next/standalone ./
COPY --from=builder /app/.next/static ./.next/static

EXPOSE 3000

CMD ["node", "server.js"]
```

建構與執行：
```bash
docker build -t codefest-app .
docker run -p 3000:3000 codefest-app
```

### 環境設定

#### Production 環境變數

在部署平台設定以下環境變數：

```bash
# 應用程式設定
NODE_ENV=production
NEXT_PUBLIC_APP_NAME=Codefest Taipei 2025
NEXT_PUBLIC_API_URL=https://api.yourdomain.com

# 資料庫（如適用）
DATABASE_URL=postgresql://user:pass@host:5432/dbname

# 第三方服務
NEXT_PUBLIC_ANALYTICS_ID=your-analytics-id
```

#### 效能最佳化設定

**next.config.ts：**
```typescript
import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  // 啟用 React Strict Mode
  reactStrictMode: true,
  
  // 壓縮
  compress: true,
  
  // 影像最佳化
  images: {
    domains: ['yourdomain.com'],
    formats: ['image/avif', 'image/webp'],
  },
  
  // 輸出設定（Docker 部署時使用）
  output: 'standalone',
};

export default nextConfig;
```

#### 監控與日誌

建議整合以下服務：
- **Vercel Analytics**：效能監控
- **Sentry**：錯誤追蹤
- **LogRocket**：使用者行為記錄

---

## 貢獻指南

### 如何提交 Issue

在提交 Issue 前，請先確認：
1. 搜尋現有 Issue，避免重複提交
2. 確認問題可以穩定重現
3. 準備好重現步驟與環境資訊

#### Issue 範本

**Bug 回報：**
```markdown
## 問題描述
簡短描述遇到的問題

## 重現步驟
1. 前往 '...'
2. 點擊 '...'
3. 捲動至 '...'
4. 看到錯誤

## 預期行為
描述應該要有的正確行為

## 實際行為
描述實際發生的錯誤行為

## 環境資訊
- OS: [例如 Windows 11]
- Browser: [例如 Chrome 120]
- Node.js: [例如 20.10.0]
- Next.js: [例如 15.5.6]

## 螢幕截圖
如適用，請附上截圖

## 其他資訊
任何其他相關資訊
```

**功能請求：**
```markdown
## 功能描述
清楚描述您想要的功能

## 問題背景
這個功能要解決什麼問題？

## 建議的解決方案
描述您理想中的實作方式

## 替代方案
描述您考慮過的其他替代方案

## 其他資訊
任何相關的範例、截圖或參考資料
```

### Pull Request 規範

#### 提交前檢查清單

- [ ] 程式碼遵循專案的程式碼風格規範
- [ ] 執行 `npm run lint` 無錯誤
- [ ] 執行 `npm run build` 建構成功
- [ ] 所有測試通過（如有）
- [ ] 更新相關文件
- [ ] Commit 訊息遵循 Conventional Commits 規範

#### PR 流程

1. **Fork 專案並建立分支**
```bash
git checkout -b feature/my-new-feature
```

2. **進行開發並提交**
```bash
git add .
git commit -m "feat: add new feature"
```

3. **推送到您的 Fork**
```bash
git push origin feature/my-new-feature
```

4. **建立 Pull Request**
- 前往原專案的 GitHub 頁面
- 點擊「New Pull Request」
- 選擇您的分支
- 填寫 PR 描述

#### PR 範本

```markdown
## 變更類型
- [ ] Bug 修復
- [ ] 新功能
- [ ] 重構
- [ ] 文件更新
- [ ] 效能改善

## 變更說明
描述這個 PR 做了什麼

## 相關 Issue
Closes #123

## 測試方式
描述如何測試這些變更

## 螢幕截圖（如適用）

## 檢查清單
- [ ] 程式碼遵循專案規範
- [ ] Lint 通過
- [ ] 建構成功
- [ ] 文件已更新
```

#### Code Review 流程

1. 至少需要 **1 位** 核心貢獻者審核
2. 所有討論串必須解決
3. CI/CD 檢查必須通過
4. 使用 Squash and Merge 合併

---

## 授權與聯絡資訊

### 授權條款

本專案採用 **MIT License** 授權。

```
MIT License

Copyright (c) 2025 Nashexplorer

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```

詳細授權內容請參閱 [LICENSE](./LICENSE) 檔案。

### 專案維護者

- **Nashexplorer** - 專案創建者與維護者
  - GitHub: [@nashexplorer](https://github.com/nashexplorer)

### 聯絡方式

- **GitHub Issues**：[提交 Issue](https://github.com/nashexplorer/2025_fall_Codefest_Taipei_Hands_On_Lab/issues)
- **GitHub Discussions**：[參與討論](https://github.com/nashexplorer/2025_fall_Codefest_Taipei_Hands_On_Lab/discussions)

### 貢獻者

感謝所有為本專案做出貢獻的開發者！

<a href="https://github.com/nashexplorer/2025_fall_Codefest_Taipei_Hands_On_Lab/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=nashexplorer/2025_fall_Codefest_Taipei_Hands_On_Lab" />
</a>

### 致謝

- [Next.js](https://nextjs.org/) - React 框架
- [Vercel](https://vercel.com/) - 部署平台
- [Tailwind CSS](https://tailwindcss.com/) - CSS 框架
- [React](https://react.dev/) - UI 函式庫
- Codefest Taipei 2025 組織團隊

### 相關連結

- [Next.js 官方文件](https://nextjs.org/docs)
- [React 官方文件](https://react.dev/)
- [Tailwind CSS 文件](https://tailwindcss.com/docs)
- [TypeScript 手冊](https://www.typescriptlang.org/docs/)
- [Vercel 部署指南](https://vercel.com/docs)

---

## 附錄

### 常見問題 (FAQ)

#### Q: 如何升級 Next.js 版本？
```bash
npm install next@latest react@latest react-dom@latest
```

#### Q: 如何啟用 TypeScript 嚴格模式？
在 `tsconfig.json` 中設定：
```json
{
  "compilerOptions": {
    "strict": true
  }
}
```

#### Q: 如何新增自訂字型？
1. 將字型檔案放入 `public/fonts/`
2. 在 `globals.css` 中定義：
```css
@font-face {
  font-family: 'CustomFont';
  src: url('/fonts/CustomFont.woff2') format('woff2');
}
```

#### Q: 如何設定深色模式？
Next.js 15 與 Tailwind CSS 4 預設支援深色模式：
```tsx
<div className="bg-white dark:bg-gray-900">
  <p className="text-black dark:text-white">Hello</p>
</div>
```

### 疑難排解

#### 建構失敗
```bash
# 清除快取
rm -rf .next node_modules
npm install
npm run build
```

#### Port 3000 已被佔用
```bash
# 使用不同 Port
npm run dev -- -p 3001
```

#### TypeScript 錯誤
```bash
# 重新生成型別定義
rm -rf .next
npm run dev
```

---

**文件版本：** 1.0.0  
**最後更新：** 2025 年 11 月 1 日  
**適用版本：** Next.js 15.5.6 | React 19.1.0 | Node.js 18+

---

> 🎉 **祝您在 Codefest Taipei 2025 中取得優異成績！**
>
> 如有任何問題或建議，歡迎透過 GitHub Issues 與我們聯繫。

