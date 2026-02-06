DENY SELECT ON SCHEMA::dbo TO [<APP_USER>];

GRANT INSERT ON OBJECT::dbo.AlumniProfiles TO [<APP_USER>];

GO
CREATE OR ALTER VIEW dbo.vw_PublicAlumniMap
AS
SELECT
    Id,
    CountryId,
    Latitude,
    Longitude
FROM dbo.AlumniProfiles
WHERE Latitude IS NOT NULL
  AND Longitude IS NOT NULL
  AND IsApproved = 1
  AND IsLocationVerified = 1;
GO

GRANT SELECT ON OBJECT::dbo.vw_PublicAlumniMap TO [<APP_USER>];

DENY UPDATE ON OBJECT::dbo.AlumniProfiles TO [<APP_USER>];
DENY DELETE ON OBJECT::dbo.AlumniProfiles TO [<APP_USER>];
