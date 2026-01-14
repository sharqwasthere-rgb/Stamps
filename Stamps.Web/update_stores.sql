ALTER TABLE "Stores" ADD COLUMN IF NOT EXISTS "Address" text;
ALTER TABLE "Stores" ADD COLUMN IF NOT EXISTS "City" text;
ALTER TABLE "Stores" ADD COLUMN IF NOT EXISTS "State" text;
ALTER TABLE "Stores" ADD COLUMN IF NOT EXISTS "PostalCode" text;
ALTER TABLE "Stores" ADD COLUMN IF NOT EXISTS "Country" text;
ALTER TABLE "Stores" ADD COLUMN IF NOT EXISTS "Latitude" double precision;
ALTER TABLE "Stores" ADD COLUMN IF NOT EXISTS "Longitude" double precision;
ALTER TABLE "Stores" ADD COLUMN IF NOT EXISTS "CreatedBy" text;
ALTER TABLE "Stores" ADD COLUMN IF NOT EXISTS "UpdatedAt" timestamp;
ALTER TABLE "Stores" ADD COLUMN IF NOT EXISTS "UpdatedBy" text;
ALTER TABLE "Stores" ADD COLUMN IF NOT EXISTS "IsDeleted" boolean DEFAULT false;
ALTER TABLE "Stores" ADD COLUMN IF NOT EXISTS "DeletedAt" timestamp;
ALTER TABLE "Stores" ADD COLUMN IF NOT EXISTS "DeletedBy" text;

