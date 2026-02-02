# Render: Fix "open Dockerfile: no such file or directory"

The Dockerfile is at the **root** of this repo (same level as Stamps.Web, Stamps.Shared).

## Required Render settings

1. **Root Directory**  
   Leave **completely empty** (clear the field; do not use `.` or `Stamps`).  
   If you set Root Directory to `Stamps`, Render builds from the MAUI app folder, where there is no Dockerfile.

2. **Dockerfile Path**  
   Set to: **`Dockerfile`**  
   (Or leave default if it is already `Dockerfile`.)

3. **Docker Build Context Directory**  
   Leave **empty** so the build context is the repo root.

Save and redeploy. The build will find `Dockerfile` at the repo root.
