# Render Deployment - Quick Setup Guide

## Step 1: Create Web Service
1. Click **"New Web Service"** on your Render dashboard
2. Connect your GitHub repository (authorize Render if needed)
3. Select your `Stamps` repository

## Step 2: Configure Settings
Since Render doesn't have a native .NET option, use **Docker**:

- **Name**: `stamps-api` (or any name you like)
- **Language**: `Docker` ✅
- **Region**: Choose closest to you (e.g., Frankfurt, Oregon)
- **Branch**: `main` (or your default branch)
- **Root Directory**: Leave EMPTY (repo root is already the Stamps folder) ✅
- **Dockerfile Path**: `Dockerfile` (or leave empty, since it's at repo root) ✅
- **Build Command**: Leave empty (Docker handles this automatically)
- **Start Command**: Leave empty (Docker handles this automatically)

**Note**: The Dockerfile is now at `Stamps/Dockerfile` (moved from `Stamps.Web/Dockerfile`).

## Step 3: Add Environment Variables
Click "Environment" tab and add:

```
DATABASE_URL = Host=aws-1-eu-central-2.pooler.supabase.com;Database=postgres;Username=postgres.extulqupjwhespvqpfal;Password=8cMuFf4BC85BVIgx;Port=6543;SSL Mode=Require;Trust Server Certificate=true

ASPNETCORE_ENVIRONMENT = Production

PORT = 8080
```

## Step 4: Deploy
1. Click "Create Web Service"
2. Wait for deployment (5-10 minutes first time)
3. Your app will be live at: `https://stamps-api.onrender.com` (or your chosen name)

## Step 5: Update Your MAUI App
Once deployed, update these files with your new Render URL:

1. **Stamps/Stamps/MauiProgram.cs** (line 39)
2. **Stamps/Stamps/Services/ApiService.cs** (line 10)

Replace: `https://byssal-janene-lyingly.ngrok-free.dev`
With: `https://your-app-name.onrender.com`

## Important Notes:
- ⚠️ Free services spin down after 15 minutes of inactivity
- First request after spin-down takes ~30 seconds (cold start)
- After that, it's fast until next inactivity period
- 750 hours/month = enough for continuous operation
