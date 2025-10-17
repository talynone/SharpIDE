using Microsoft.CodeAnalysis.Classification;
using Microsoft.CodeAnalysis.Text;

namespace SharpIDE.Application.Features.Analysis;

public readonly record struct SharpIdeClassifiedSpan(LinePositionSpan FileSpan, ClassifiedSpan ClassifiedSpan);
