using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EarlyLearner.Worker.Messaging;

/// <summary>
/// Used as an anchor point to register all consumers within the
/// same namespace instead of individually registering them.
/// </summary>
internal sealed record ConsumerAnchor();


