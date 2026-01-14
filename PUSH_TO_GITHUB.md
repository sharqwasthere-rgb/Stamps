# Push Code to GitHub - Quick Guide

## Step 1: Install Git (if not installed)
If git is not installed, run:
```powershell
winget install Git.Git
```

Then restart your terminal.

## Step 2: Initialize Git Repository
Open PowerShell in the `Stamps` folder and run:

```powershell
cd C:\Users\espoir.benissanh\source\repos\Stamps\Stamps
git init
git branch -M main
```

## Step 3: Add All Files
```powershell
git add .
```

## Step 4: Make First Commit
```powershell
git commit -m "Initial commit - Stamps loyalty card app"
```

## Step 5: Add Remote Repository
Replace `sharqwasthere-rgb` with your GitHub username if different:

```powershell
git remote add origin https://github.com/sharqwasthere-rgb/Stamps.git
```

## Step 6: Push to GitHub
```powershell
git push -u origin main
```

You'll be prompted for your GitHub username and password (use a Personal Access Token, not your password).

---

## If You Need a Personal Access Token:
1. Go to GitHub.com → Settings → Developer settings → Personal access tokens → Tokens (classic)
2. Click "Generate new token (classic)"
3. Give it a name like "Stamps App"
4. Select scopes: `repo` (full control)
5. Click "Generate token"
6. Copy the token and use it as your password when pushing

---

## After Pushing:
Once your code is on GitHub, go back to Render and:
1. Click "New Web Service"
2. Connect your GitHub account
3. Select the `Stamps` repository
4. Follow the Render setup guide!
