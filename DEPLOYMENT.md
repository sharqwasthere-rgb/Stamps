# Free Hosting Deployment Guide

## ⚠️ Free Tier Limitations:

- **Railway**: 30-day trial with $5, then $1/month (very limited)
- **Render**: 750 hours/month (≈31 days) - **FREE FOREVER** ✅
- **Fly.io**: Free tier with generous limits - **FREE FOREVER** ✅
- **Azure**: Free F1 tier - **FREE FOREVER** ✅

---

## Option 1: Render (Recommended - Truly Free Forever)

**Free Tier**: 750 hours/month (enough for continuous operation)

### Steps:
1. Go to [render.com](https://render.com) and sign up
2. Click "New" → "Web Service"
3. Connect your GitHub repository
4. Render will auto-detect `render.yaml` file
5. Add environment variable:
   - `DATABASE_URL` = Your Supabase connection string
6. Deploy!

Your app will get a URL like: `https://your-app-name.onrender.com`

**Note**: Free services spin down after 15 minutes of inactivity, but wake up on first request (takes ~30 seconds).

---

## Option 2: Fly.io (Truly Free Forever)

**Free Tier**: 3 shared-cpu VMs, 3GB persistent volumes

### Steps:
1. Install Fly CLI: `winget install flyctl` (or download from fly.io)
2. Run: `flyctl auth login`
3. In project root, run: `flyctl launch`
4. Follow prompts (don't create a database, we use Supabase)
5. Set secret: `flyctl secrets set DATABASE_URL="your-connection-string"`
6. Deploy: `flyctl deploy`

Your app will get a URL like: `https://your-app-name.fly.dev`

---

## Option 3: Azure App Service (Truly Free Forever)

**Free Tier**: F1 (Free) - Always free, no time limit

### Steps:
1. Go to [portal.azure.com](https://portal.azure.com) and sign up (free account)
2. Create "App Service" → "Web App"
3. Choose "Free (F1)" pricing tier
4. Connect to GitHub for deployment
5. Add Application Setting:
   - `DATABASE_URL` = Your Supabase connection string
6. Deploy!

Your app will get a URL like: `https://your-app-name.azurewebsites.net`

---

## Option 4: Railway (Limited - 30 days then $1/month)

**Free Tier**: 30-day trial with $5, then $1/month (very limited)

### Steps:
1. Go to [railway.app](https://railway.app) and sign up
2. Click "New Project" → "Deploy from GitHub repo"
3. Connect your GitHub repository
4. Railway will auto-detect the Dockerfile
5. Add environment variables:
   - `DATABASE_URL` = Your Supabase connection string
   - `ASPNETCORE_ENVIRONMENT` = `Production`
6. Deploy!

Your app will get a URL like: `https://your-app-name.up.railway.app`

---

## After Deployment:

1. Update your MAUI app's API URL in:
   - `Stamps/Stamps/MauiProgram.cs` (line 39)
   - `Stamps/Stamps/Services/ApiService.cs` (line 10)

2. Replace `https://byssal-janene-lyingly.ngrok-free.dev` with your new URL

3. Rebuild and deploy your MAUI app

---

## Environment Variables Needed:

```
DATABASE_URL=Host=aws-1-eu-central-2.pooler.supabase.com;Database=postgres;Username=postgres.extulqupjwhespvqpfal;Password=8cMuFf4BC85BVIgx;Port=6543;SSL Mode=Require;Trust Server Certificate=true

ASPNETCORE_ENVIRONMENT=Production
```
