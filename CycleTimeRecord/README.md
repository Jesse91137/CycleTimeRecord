# CycleTimeRecord 專案文件

## 專案概述

CycleTimeRecord 是一個使用 ASP.NET MVC 技術開發的 Web 應用程式，主要用於記錄和管理製程的週期時間 (Cycle Time)。專案基於 .NET Framework 4.7.2，並使用 Entity Framework 作為資料庫存取層。

### 技術堆疊

*   **前端**:
    *   HTML
    *   CSS
    *   JavaScript
    *   [Bootstrap](https://getbootstrap.com/) (v3.4.1) - 用於快速建立響應式網頁佈局。
    *   [jQuery](https://jquery.com/) (v3.4.1) - 用於簡化 DOM 操作和 AJAX 請求。
    *   [jQuery Validation](https://jqueryvalidation.org/) (v1.17.0) - 用於前端表單驗證。
    *   [Modernizr](https://modernizr.com/) (v2.8.3) - 用於檢測瀏覽器對 HTML5 和 CSS3 功能的支援。
    *   [PagedList](https://github.com/troygoode/PagedList) - 分頁

*   **後端**:
    *   C#
    *   [ASP.NET MVC](https://dotnet.microsoft.com/apps/aspnet/mvc) (v5.2.7) - 用於構建 Web 應用程式的框架。
    *   [Entity Framework](https://docs.microsoft.com/en-us/ef/) (v6.1.3) - 用於資料庫存取。
    *   [Newtonsoft.Json](https://www.newtonsoft.com/json) (v12.0.2) - 用於處理 JSON 資料。
    *   [WebGrease](https://github.com/madskristensen/WebGrease) (v1.6.0) - 用於優化 JavaScript 和 CSS 檔案。
    *   [Antlr](https://www.antlr.org/) (v3.5.0.2) - 用於語法分析。

*   **開發工具**:
    *   [Microsoft.CodeDom.Providers.DotNetCompilerPlatform](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codedom.providers.dotnetcompilerplatform) (v2.0.1) - 提供對 Roslyn 編譯器的支援。

### 資料庫

專案使用 Entity Framework 作為 ORM 框架，連接到 SQL Server 資料庫。具體的資料庫連接字串配置在 `Web.config` 檔案中。

## 架構說明

CycleTimeRecord 專案採用典型的 ASP.NET MVC 架構，分為三個主要層次：

*   **模型 (Models)**：負責資料庫存取和業務邏輯。
    *   使用 Entity Framework (Code First) 與 SQL Server 資料庫互動。
    *   主要的實體類別包括：`CT_CycleTimeRecord_Role`、`CT_LineWork`、`CT_Member`、`CT_MemberRole`、`CTWorks`。
    *   `CycleTimeDb.Context.cs` 定義了資料庫上下文。

*   **視圖 (Views)**：負責呈現使用者介面。
    *   使用 Razor 引擎 (.cshtml) 建立 HTML 頁面。
    *   視圖按照功能模組組織在不同的資料夾中，與控制器對應。
    *   `Shared` 資料夾包含共用的佈局和錯誤頁面。

*   **控制器 (Controllers)**：負責處理使用者請求，調用模型層的邏輯，並選擇適當的視圖呈現給使用者。
    *   `CycleTimeController`：處理與週期時間記錄相關的操作。
    *   `HomeController`：處理應用程式的首頁和相關請求。
    *   `MaintainController`：處理系統維護相關的功能。

**資料流程**:

1.  使用者通過瀏覽器發送請求。
2.  ASP.NET MVC 路由引擎根據 URL 將請求分配給相應的控制器和動作。
3.  控制器調用模型層的程式碼來處理業務邏輯，例如從資料庫中讀取或寫入資料。
4.  模型層使用 Entity Framework 與資料庫互動。
5.  控制器將模型資料傳遞給視圖。
6.  視圖使用 Razor 引擎將模型資料渲染成 HTML 頁面。
7.  將 HTML 頁面返回給瀏覽器，呈現給使用者。

## 模組說明

### CycleTimeController

`CycleTimeController` 負責處理與週期時間記錄相關的操作，包括：

*   **`CTWork`**: 顯示週期時間作業的主要頁面。
    *   驗證使用者登入。
    *   獲取週期時間資料。
    *   計算優化百分比。
    *   分頁顯示資料。
    *   支援 AJAX 請求，返回部分視圖。
*   **`CTWorkPartial`**: 載入用於 AJAX 更新的部分視圖。
*   **`CTWorkAjax`**: 根據線別、機種名稱和板面進行 AJAX 查詢。
*   **`CTWorkInHistory`**: 更新瓶頸工時，並將資料寫入歷史記錄。
    *   調用 `TxStdTimeAsync` 方法，更新 AMES 系統的標準工時。
*   **`CTWorkSearch`**: 根據線別、機種名稱、板面和日期範圍進行查詢。
*   **`DeleteCT`**: 刪除週期時間記錄。
*   **`Login_Authentication`**: 驗證使用者登入狀態

### HomeController

`HomeController` 通常是應用程式的首頁和入口點。它可能包含以下功能：

*   **顯示應用程式簡介**：介紹 CycleTimeRecord 應用程式的功能和用途。
*   **提供導航**：提供到其他功能模組的連結。
*   **顯示登入/登出連結**：如果應用程式需要身份驗證，則提供登入和登出連結。

### MaintainController

`MaintainController` 負責系統維護相關的功能。它可能包含以下功能：

*   **使用者管理**：允許管理員新增、修改和刪除使用者帳戶。
*   **權限設定**：允許管理員設定不同使用者的權限。
*   **系統設定**：允許管理員配置應用程式的一些設定。
*   **資料備份/還原**: 允許備份和還原資料庫

## 安裝/部署說明

### 先決條件

*   .NET Framework 4.7.2
*   SQL Server 資料庫
*   IIS (Internet Information Services)
*   AMES 服務 (位於 `http://192.168.4.200/AMES/Service/`)

### 部署步驟

1.  **建立資料庫**:
    *   在 SQL Server 中建立一個名為 `CycleTimeRecord` 的資料庫。
    *   建立一個名為 `CT_Admin` 的使用者，密碼設定為 `CT12345678`，並授予該使用者對 `CycleTimeRecord` 資料庫的適當權限。
    *   執行 Entity Framework 的 Migration 腳本，或使用 Code First 的 `Update-Database` 命令，建立資料庫表格。

2.  **配置 Web.config**:
    *   根據實際部署環境，修改 `Web.config` 中的 `connectionStrings` 部分。
        *   如果是部署到正式區，請確保使用正式區的資料庫連接字串：
            ```xml
            <add name="CycleTimeRecord" connectionString="data source=192.168.7.39;initial catalog=CycleTimeRecord;User ID=CT_Admin;Password=CT12345678;MultipleActiveResultSets=True;App=EntityFramework" providerName="System.Data.SqlClient" />
            <add name="CycleTimeRecordEntities" connectionString="metadata=res://*/Models.CycleTimeDb.csdl|res://*/Models.CycleTimeDb.ssdl|res://*/Models.CycleTimeDb.msl;provider=System.Data.SqlClient;provider connection string="data source=192.168.7.39;initial catalog=CycleTimeRecord;user id=CT_Admin;password=CT12345678;multipleactiveresultsets=True;application name=EntityFramework"" providerName="System.Data.EntityClient" />
            ```
        *   如果是部署到開發環境，可以使用註解掉的開發端設定，並修改為正確的本地資料庫資訊。

3.  **發佈網站**:
    *   使用 Visual Studio 的「發佈」功能，將專案發佈到 IIS。
    *   在 IIS 中建立一個網站或應用程式，指向發佈後的檔案。

4.  **確認 AMES 服務可訪問**:
    *   確保應用程式可以訪問 AMES 服務 (`http://192.168.4.200/AMES/Service/`)。

### 注意事項

*   `Web.Release.config` 檔案中移除了 `debug` 屬性 (<compilation xdt:Transform="RemoveAttributes(debug)" />)，這表示在 Release 模式下，將禁用偵錯模式。

## 使用說明
此應用程式主要功能為週期時間記錄，使用者可透過網頁介面進行新增、查詢、修改、刪除週期時間等操作。
管理員可透過維護功能進行使用者管理、權限設定、系統設定和資料備份/還原。
