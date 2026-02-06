GO
ALTER TABLE dbo.AlumniProfiles
ADD ContactEmail_Normalized AS LOWER(LTRIM(RTRIM(ContactEmail))) PERSISTED;
GO

CREATE UNIQUE INDEX UX_AlumniProfiles_ContactEmail_Normalized
ON dbo.AlumniProfiles (ContactEmail_Normalized);
GO

ALTER TABLE dbo.AlumniProfiles
ADD CONSTRAINT CK_AlumniProfiles_Latitude
CHECK (Latitude IS NULL OR (Latitude >= -90 AND Latitude <= 90));
GO

ALTER TABLE dbo.AlumniProfiles
ADD CONSTRAINT CK_AlumniProfiles_Longitude
CHECK (Longitude IS NULL OR (Longitude >= -180 AND Longitude <= 180));
GO

ALTER TABLE dbo.AlumniProfiles
ADD CONSTRAINT CK_AlumniProfiles_Email_Basic
CHECK (
  ContactEmail IS NOT NULL
  AND LEN(LTRIM(RTRIM(ContactEmail))) <= 256
  AND ContactEmail LIKE '%_@_%._%'
);
GO

ALTER TABLE dbo.AlumniProfiles
ADD CONSTRAINT CK_AlumniProfiles_DateOfBirth
CHECK (DateOfBirth >= '1900-01-01' AND DateOfBirth <= CONVERT(date, GETUTCDATE()));
GO

ALTER TABLE dbo.AlumniProfiles
ADD CONSTRAINT CK_AlumniProfiles_GraduationDate
CHECK (GraduationDate >= '1900-01-01' AND GraduationDate <= DATEADD(year, 10, CONVERT(date, GETUTCDATE())));
GO

CREATE INDEX IX_AlumniProfiles_MapPoints
ON dbo.AlumniProfiles (IsApproved, IsLocationVerified)
INCLUDE (Latitude, Longitude, CountryId);
GO
