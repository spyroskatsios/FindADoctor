﻿namespace Doctors.Application.Common.Models;

public record CurrentUser(string Id, IEnumerable<string> Permissions, IEnumerable<string> Roles);