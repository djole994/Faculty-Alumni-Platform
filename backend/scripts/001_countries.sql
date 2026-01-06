-- backend/scripts/seed/001_countries.sql
SET NOCOUNT ON;

-- CREATE UNIQUE INDEX UX_Countries_IsoCode ON dbo.Countries(IsoCode);

INSERT INTO dbo.Countries (Name, IsoCode, DefaultLatitude, DefaultLongitude)
SELECT v.Name, v.IsoCode, v.DefaultLatitude, v.DefaultLongitude
FROM (VALUES 
-- A
('Afghanistan', 'AF', 33.9391, 67.7100),
('Albania', 'AL', 41.1533, 20.1683),
('Algeria', 'DZ', 28.0339, 1.6596),
('Andorra', 'AD', 42.5063, 1.5218),
('Angola', 'AO', -11.2027, 17.8739),
('Antigua and Barbuda', 'AG', 17.0608, -61.7964),
('Argentina', 'AR', -38.4161, -63.6167),
('Armenia', 'AM', 40.0691, 45.0382),
('Australia', 'AU', -25.2744, 133.7751),
('Austria', 'AT', 47.5162, 14.5501),
('Azerbaijan', 'AZ', 40.1431, 47.5769),

-- B
('Bahamas', 'BS', 25.0343, -77.3963),
('Bahrain', 'BH', 26.0667, 50.5577),
('Bangladesh', 'BD', 23.6850, 90.3563),
('Barbados', 'BB', 13.1939, -59.5432),
('Belarus', 'BY', 53.7098, 27.9534),
('Belgium', 'BE', 50.5039, 4.4699),
('Belize', 'BZ', 17.1899, -88.4976),
('Benin', 'BJ', 9.3077, 2.3158),
('Bhutan', 'BT', 27.5142, 90.4336),
('Bolivia', 'BO', -16.2902, -63.5887),
('Bosnia and Herzegovina', 'BA', 43.9159, 17.6791),
('Botswana', 'BW', -22.3285, 24.6849),
('Brazil', 'BR', -14.2350, -51.9253),
('Brunei', 'BN', 4.5353, 114.7277),
('Bulgaria', 'BG', 42.7339, 25.4858),
('Burkina Faso', 'BF', 12.2383, -1.5616),
('Burundi', 'BI', -3.3731, 29.9189),

-- C
('Cabo Verde', 'CV', 16.0021, -24.0131),
('Cambodia', 'KH', 12.5657, 104.9910),
('Cameroon', 'CM', 7.3697, 12.3547),
('Canada', 'CA', 56.1304, -106.3468),
('Central African Republic', 'CF', 6.6111, 20.9394),
('Chad', 'TD', 15.4542, 18.7322),
('Chile', 'CL', -35.6751, -71.5430),
('China', 'CN', 35.8617, 104.1954),
('Colombia', 'CO', 4.5709, -74.2973),
('Comoros', 'KM', -11.8750, 43.8722),
('Congo (Congo-Brazzaville)', 'CG', -0.2280, 15.8277),
('Costa Rica', 'CR', 9.7489, -83.7534),
('Croatia', 'HR', 45.1000, 15.2000),
('Cuba', 'CU', 21.5218, -77.7812),
('Cyprus', 'CY', 35.1264, 33.4299),
('Czechia (Czech Republic)', 'CZ', 49.8175, 15.4730),

-- D
('Democratic Republic of the Congo', 'CD', -4.0383, 21.7587),
('Denmark', 'DK', 56.2639, 9.5018),
('Djibouti', 'DJ', 11.8251, 42.5903),
('Dominica', 'DM', 15.4150, -61.3710),
('Dominican Republic', 'DO', 18.7357, -70.1627),

-- E
('Ecuador', 'EC', -1.8312, -78.1834),
('Egypt', 'EG', 26.8206, 30.8025),
('El Salvador', 'SV', 13.7942, -88.8965),
('Equatorial Guinea', 'GQ', 1.6508, 10.2679),
('Eritrea', 'ER', 15.1794, 39.7823),
('Estonia', 'EE', 58.5953, 25.0136),
('Eswatini', 'SZ', -26.5225, 31.4659),
('Ethiopia', 'ET', 9.1450, 40.4897),

-- F
('Fiji', 'FJ', -17.7134, 178.0650),
('Finland', 'FI', 61.9241, 25.7482),
('France', 'FR', 46.2276, 2.2137),

-- G
('Gabon', 'GA', -0.8037, 11.6094),
('Gambia', 'GM', 13.4432, -15.3101),
('Georgia', 'GE', 42.3154, 43.3569),
('Germany', 'DE', 51.1657, 10.4515),
('Ghana', 'GH', 7.9465, -1.0232),
('Greece', 'GR', 39.0742, 21.8243),
('Grenada', 'GD', 12.1165, -61.6790),
('Guatemala', 'GT', 15.7835, -90.2308),
('Guinea', 'GN', 9.9456, -9.6966),
('Guinea-Bissau', 'GW', 11.8037, -15.1804),
('Guyana', 'GY', 4.8604, -58.9302),

-- H
('Haiti', 'HT', 18.9712, -72.2852),
('Honduras', 'HN', 15.2000, -86.2419),
('Hungary', 'HU', 47.1625, 19.5033),

-- I
('Iceland', 'IS', 64.9631, -19.0208),
('India', 'IN', 20.5937, 78.9629),
('Indonesia', 'ID', -0.7893, 113.9213),
('Iran', 'IR', 32.4279, 53.6880),
('Iraq', 'IQ', 33.2232, 43.6793),
('Ireland', 'IE', 53.1424, -7.6921),
('Israel', 'IL', 31.0461, 34.8516),
('Italy', 'IT', 41.8719, 12.5674),
('Ivory Coast', 'CI', 7.5400, -5.5471),

-- J
('Jamaica', 'JM', 18.1096, -77.2975),
('Japan', 'JP', 36.2048, 138.2529),
('Jordan', 'JO', 30.5852, 36.2384),

-- K
('Kazakhstan', 'KZ', 48.0196, 66.9237),
('Kenya', 'KE', -0.0236, 37.9062),
('Kiribati', 'KI', -3.3704, -168.7340),
('Kosovo', 'XK', 42.6026, 20.9030),
('Kuwait', 'KW', 29.3117, 47.4818),
('Kyrgyzstan', 'KG', 41.2044, 74.7661),

-- L
('Laos', 'LA', 19.8563, 102.4955),
('Latvia', 'LV', 56.8796, 24.6032),
('Lebanon', 'LB', 33.8547, 35.8623),
('Lesotho', 'LS', -29.6099, 28.2336),
('Liberia', 'LR', 6.4281, -9.4295),
('Libya', 'LY', 26.3351, 17.2283),
('Liechtenstein', 'LI', 47.1660, 9.5554),
('Lithuania', 'LT', 55.1694, 23.8813),
('Luxembourg', 'LU', 49.8153, 6.1296),

-- M
('Madagascar', 'MG', -18.7669, 46.8691),
('Malawi', 'MW', -13.2543, 34.3015),
('Malaysia', 'MY', 4.2105, 101.9758),
('Maldives', 'MV', 3.2028, 73.2207),
('Mali', 'ML', 17.5707, -3.9962),
('Malta', 'MT', 35.9375, 14.3754),
('Marshall Islands', 'MH', 7.1315, 171.1845),
('Mauritania', 'MR', 21.0079, -10.9408),
('Mauritius', 'MU', -20.3484, 57.5522),
('Mexico', 'MX', 23.6345, -102.5528),
('Micronesia', 'FM', 7.4256, 150.5508),
('Moldova', 'MD', 47.4116, 28.3699),
('Monaco', 'MC', 43.7384, 7.4246),
('Mongolia', 'MN', 46.8625, 103.8467),
('Montenegro', 'ME', 42.7087, 19.3744),
('Morocco', 'MA', 31.7917, -7.0926),
('Mozambique', 'MZ', -18.6657, 35.5296),
('Myanmar', 'MM', 21.9162, 95.9560),

-- N
('Namibia', 'NA', -22.9576, 18.4904),
('Nauru', 'NR', -0.5228, 166.9315),
('Nepal', 'NP', 28.3949, 84.1240),
('Netherlands', 'NL', 52.1326, 5.2913),
('New Zealand', 'NZ', -40.9006, 174.8860),
('Nicaragua', 'NI', 12.8654, -85.2072),
('Niger', 'NE', 17.6078, 8.0817),
('Nigeria', 'NG', 9.0820, 8.6753),
('North Korea', 'KP', 40.3399, 127.5101),
('North Macedonia', 'MK', 41.6086, 21.7453),
('Norway', 'NO', 60.4720, 8.4689),

-- O
('Oman', 'OM', 21.5126, 55.9233),

-- P
('Pakistan', 'PK', 30.3753, 69.3451),
('Palau', 'PW', 7.5150, 134.5825),
('Palestine', 'PS', 31.9522, 35.2332),
('Panama', 'PA', 8.5380, -80.7821),
('Papua New Guinea', 'PG', -6.3150, 143.9555),
('Paraguay', 'PY', -23.4425, -58.4438),
('Peru', 'PE', -9.1900, -75.0152),
('Philippines', 'PH', 12.8797, 121.7740),
('Poland', 'PL', 51.9194, 19.1451),
('Portugal', 'PT', 39.3999, -8.2245),

-- Q
('Qatar', 'QA', 25.3548, 51.1839),

-- R
('Romania', 'RO', 45.9432, 24.9668),
('Russia', 'RU', 61.5240, 105.3188),
('Rwanda', 'RW', -1.9403, 29.8739),

-- S
('Saint Kitts and Nevis', 'KN', 17.3578, -62.7830),
('Saint Lucia', 'LC', 13.9094, -60.9789),
('Saint Vincent and the Grenadines', 'VC', 12.9843, -61.2872),
('Samoa', 'WS', -13.7590, -172.1046),
('San Marino', 'SM', 43.9424, 12.4578),
('Sao Tome and Principe', 'ST', 0.1864, 6.6131),
('Saudi Arabia', 'SA', 23.8859, 45.0792),
('Senegal', 'SN', 14.4974, -14.4524),
('Serbia', 'RS', 44.0165, 21.0059),
('Seychelles', 'SC', -4.6796, 55.4920),
('Sierra Leone', 'SL', 8.4606, -11.7799),
('Singapore', 'SG', 1.3521, 103.8198),
('Slovakia', 'SK', 48.6690, 19.6990),
('Slovenia', 'SI', 46.1512, 14.9955),
('Solomon Islands', 'SB', -9.6457, 160.1562),
('Somalia', 'SO', 5.1521, 46.1996),
('South Africa', 'ZA', -30.5595, 22.9375),
('South Korea', 'KR', 35.9078, 127.7669),
('South Sudan', 'SS', 6.8770, 31.3070),
('Spain', 'ES', 40.4637, -3.7492),
('Sri Lanka', 'LK', 7.8731, 80.7718),
('Sudan', 'SD', 12.8628, 30.2176),
('Suriname', 'SR', 3.9193, -56.0278),
('Sweden', 'SE', 62.0000, 15.0000),
('Switzerland', 'CH', 46.8182, 8.2275),
('Syria', 'SY', 34.8021, 38.9968),

-- T
('Taiwan', 'TW', 23.6978, 120.9605),
('Tajikistan', 'TJ', 38.8610, 71.2761),
('Tanzania', 'TZ', -6.3690, 34.8888),
('Thailand', 'TH', 15.8700, 100.9925),
('Timor-Leste', 'TL', -8.8742, 125.7275),
('Togo', 'TG', 8.6195, 0.8248),
('Tonga', 'TO', -21.1790, -175.1982),
('Trinidad and Tobago', 'TT', 10.6918, -61.2225),
('Tunisia', 'TN', 33.8869, 9.5375),
('Turkey', 'TR', 38.9637, 35.2433),
('Turkmenistan', 'TM', 38.9697, 59.5563),
('Tuvalu', 'TV', -7.1095, 177.6493),

-- U
('Uganda', 'UG', 1.3733, 32.2903),
('Ukraine', 'UA', 48.3794, 31.1656),
('United Arab Emirates', 'AE', 23.4241, 53.8478),
('United Kingdom', 'GB', 55.3781, -3.4360),
('United States', 'US', 37.0902, -95.7129),
('Uruguay', 'UY', -32.5228, -55.7658),
('Uzbekistan', 'UZ', 41.3775, 64.5853),

-- V
('Vanuatu', 'VU', -15.3767, 166.9592),
('Vatican City', 'VA', 41.9029, 12.4534),
('Venezuela', 'VE', 6.4238, -66.5897),
('Vietnam', 'VN', 14.0583, 108.2772),

-- Y
('Yemen', 'YE', 15.5527, 48.5164),

-- Z
('Zambia', 'ZM', -13.1339, 27.8493),
('Zimbabwe', 'ZW', -19.0154, 29.1549)) v(Name, IsoCode, DefaultLatitude, DefaultLongitude)
WHERE NOT EXISTS (
  SELECT 1 FROM dbo.Countries c WHERE c.IsoCode = v.IsoCode
);
