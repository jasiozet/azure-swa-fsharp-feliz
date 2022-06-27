# Azure Static Web Apps F#6/.NET 6 Feliz Template

This repository contains a template for creating an [Azure Static Web Apps](https://docs.microsoft.com/azure/static-web-apps/?WT.mc_id=dotnet-33392-aapowell) projects using Feliz and F# Azure Functions.

Loosely based on: [swa-feliz-template](https://github.com/aaronpowell/swa-feliz-template)

TODO:
- Feliz.DaisyUi
- Tailwind
- Better configuration for Vite

To get started clone the repository.

## Running The Application - Manual Environment Setup

Dependencies:

* .NET SDK 6
* Node.js 14
* [Azure Static Web Apps CLI](https://github.com/azure/static-web-apps-cli)
* [Azure Function Core Tools](https://github.com/Azure/azure-functions-core-tools)

Once the repo is created from the terminal run:

```bash
$> dotnet tool restore
$> npm install
$> npm install -g @azure/static-web-apps-cli azure-functions-core-tools@4
```

With all dependencies installed, you can launch the apps with a single command:

`swa start`

Then you can navigate to `http://localhost:4280` to access the emulator.

## Deploying to Static Web Apps (TODO)

To deploy a site on [Azure Static Web Apps](https://docs.microsoft.com/azure/static-web-apps/?WT.mc_id=dotnet-33392-aapowell) using this template, you'll need to customise the build pipeline that is generated for you to build the web app outside of the Static Web Apps task (the Azure Function componet will build fine). This is because it does not detect that the project needs both Node.js and .NET installed, resulting in only Node.js being installed.

Here is a sample `build_and_deploy_job`:

```yml
obs:
  build_and_deploy_job:
    if: github.event_name == 'push' || (github.event_name == 'pull_request' && github.event.action != 'closed')
    runs-on: ubuntu-latest
    name: Build and Deploy Job
    steps:
      - uses: actions/checkout@v2
        with:
          submodules: true
      - uses: actions/setup-dotnet@v1
        with:
          dotnet-version: '6.0.x'
      - uses: actions/setup-node@v1
        with:
          node-version: 14
      - run: |
          dotnet tool restore
          dotnet paket install
          npm ci
          npm run build
      - name: Build And Deploy
        id: builddeploy
        uses: Azure/static-web-apps-deploy@v1
        env:
          PRE_BUILD_COMMAND: dotnet tool restore
        with:
          azure_static_web_apps_api_token: <SECRET HERE>
          repo_token: ${{ secrets.GITHUB_TOKEN }} # Used for Github integrations (i.e. PR comments)
          action: "upload"
          ###### Repository/Build Configurations - These values can be configured to match your app requirements. ######
          # For more information regarding Static Web App workflow configurations, please visit: https://aka.ms/swaworkflowconfig
          app_location: "/public" # App source code path
          api_location: "api" # Api source code path - optional
          output_location: "/" # Built app content directory - optional
          ###### End of Repository/Build Configurations ######
```

The main things of note are:

* .NET and Node are installed on the runner
* The Fable/UI component is built before the SWA Action is used
* SWA sees the output folder, `/public` as the fully compiled app and skips the build step
* `dotnet tool restore` is done as a `PRE_BUILD_COMMAND` to ensure `paket` is available when building Azure Functions
