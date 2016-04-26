SELECT PrincipalStringIdentifier, Name, Value
FROM [Principal]
INNER JOIN [Property]
ON Principal.PrincipalId=Property.PricipalId;