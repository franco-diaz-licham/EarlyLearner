using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EarlyLearner.Shared.Utilities;

/// <summary>
/// Enumerates the possible result types for an operation, indicating its outcome or status.
/// Used to standardize responses across the application and API layers.
/// </summary>
public enum ResultTypeEnum
{
    Success,
    Updated,
    Created,
    NotFound,
    Invalid,
    Unauthorized,
    Forbidden,
    Conflict,
    Unprocessable,
    InvalidState,
    Unexpected
}

