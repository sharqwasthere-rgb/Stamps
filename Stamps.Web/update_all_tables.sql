-- Add missing columns to StampCards
ALTER TABLE "StampCards" ADD COLUMN IF NOT EXISTS "CreatedAt" timestamp DEFAULT NOW();
ALTER TABLE "StampCards" ADD COLUMN IF NOT EXISTS "CreatedBy" text;
ALTER TABLE "StampCards" ADD COLUMN IF NOT EXISTS "UpdatedAt" timestamp;
ALTER TABLE "StampCards" ADD COLUMN IF NOT EXISTS "UpdatedBy" text;
ALTER TABLE "StampCards" ADD COLUMN IF NOT EXISTS "IsDeleted" boolean DEFAULT false;
ALTER TABLE "StampCards" ADD COLUMN IF NOT EXISTS "DeletedAt" timestamp;
ALTER TABLE "StampCards" ADD COLUMN IF NOT EXISTS "DeletedBy" text;

-- Add missing columns to Transactions
ALTER TABLE "Transactions" ADD COLUMN IF NOT EXISTS "CreatedAt" timestamp DEFAULT NOW();
ALTER TABLE "Transactions" ADD COLUMN IF NOT EXISTS "CreatedBy" text;
ALTER TABLE "Transactions" ADD COLUMN IF NOT EXISTS "UpdatedAt" timestamp;
ALTER TABLE "Transactions" ADD COLUMN IF NOT EXISTS "UpdatedBy" text;
ALTER TABLE "Transactions" ADD COLUMN IF NOT EXISTS "IsDeleted" boolean DEFAULT false;
ALTER TABLE "Transactions" ADD COLUMN IF NOT EXISTS "DeletedAt" timestamp;
ALTER TABLE "Transactions" ADD COLUMN IF NOT EXISTS "DeletedBy" text;

-- Add missing columns to QRTokens
ALTER TABLE "QRTokens" ADD COLUMN IF NOT EXISTS "CreatedAt" timestamp DEFAULT NOW();
ALTER TABLE "QRTokens" ADD COLUMN IF NOT EXISTS "CreatedBy" text;
ALTER TABLE "QRTokens" ADD COLUMN IF NOT EXISTS "UpdatedAt" timestamp;
ALTER TABLE "QRTokens" ADD COLUMN IF NOT EXISTS "UpdatedBy" text;
ALTER TABLE "QRTokens" ADD COLUMN IF NOT EXISTS "IsDeleted" boolean DEFAULT false;
ALTER TABLE "QRTokens" ADD COLUMN IF NOT EXISTS "DeletedAt" timestamp;
ALTER TABLE "QRTokens" ADD COLUMN IF NOT EXISTS "DeletedBy" text;

