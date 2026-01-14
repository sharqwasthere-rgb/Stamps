# Render Settings - EXACT CONFIGURATION

## Critical Settings (Copy These Exactly):

### Basic Settings:
- **Name**: `stamps-api` (or any name)
- **Language**: `Docker`
- **Branch**: `main`
- **Region**: `Frankfurt (EU Central)` (or your preferred region)

### Docker Settings:
- **Root Directory**: Leave EMPTY (blank)
- **Docker Build Context Directory**: Leave EMPTY (blank)
- **Dockerfile Path**: `Dockerfile` (exactly this, no quotes, no slashes)
- **Docker Command**: Leave EMPTY
- **Pre-Deploy Command**: Leave EMPTY

### Environment Variables:
Add these in the "Environment" tab:

1. **Name**: `DATABASE_URL`
   **Value**: `Host=aws-1-eu-central-2.pooler.supabase.com;Database=postgres;Username=postgres.extulqupjwhespvqpfal;Password=8cMuFf4BC85BVIgx;Port=6543;SSL Mode=Require;Trust Server Certificate=true`

2. **Name**: `ASPNETCORE_ENVIRONMENT`
   **Value**: `Production`

3. **Name**: `PORT`
   **Value**: `8080`

---

## If Deployment Still Fails:

1. **Delete the service** in Render
2. **Verify Dockerfile exists** at repository root (should be at `Stamps/Dockerfile` in GitHub)
3. **Create a new service** with the exact settings above
4. **Check build logs** for specific errors

---

## Repository Structure (for reference):
```
Stamps/ (repo root)
  ├── Dockerfile ✅ (this is what Render needs)
  ├── Stamps.Web/
  ├── Stamps.Shared/
  └── ...
```
