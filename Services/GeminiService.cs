
using Google.GenAI;
using Microsoft.Extensions.Configuration;

namespace CrumbCount2.Services
{
    public class GeminiService
    {
        private readonly Client _client;

        public GeminiService(IConfiguration configuration)
        {
            var apiKey = configuration["GEMINI_API_KEY"];

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException(
                    "Gemini API key is missing from configuration.");
            }

            _client = new Client(apiKey: apiKey);
        }

        public async Task<string> GenerateWasteInsights(
            string reportData,
            string holidayData)
        {
            string prompt = $@"
You are an AI assistant for CrumbCount,
a bakery waste tracking and cost analysis system.

Analyze the following bakery waste report:

{reportData}

Provide your analysis using these four sections:

1. KEY OBSERVATIONS

Explain the important findings.
Mention the total records, quantity wasted, and estimated waste cost.
Use only the numerical values provided in the report.

2. HIGH WASTE PRODUCTS AND CATEGORIES

Identify the products with the highest recorded waste quantities
and waste costs.

Identify the categories with the highest recorded waste costs.

If there is only one product or category, state that clearly.
Do not confuse the product with the category.

3. COMMON REASONS FOR WASTE

Identify every distinct recorded reason for waste.

Explain which reason accounts for the highest quantity and which
accounts for the highest waste cost, based on the supplied summaries.

Do not omit any recorded reason.

4. RECOMMENDATIONS

IMPORTANT RECOMMENDATION RULES:

- Provide at least one relevant recommendation for EVERY distinct
  waste reason appearing in the report.
- Do not focus only on the reason with the highest quantity or cost.
- Connect each recommendation to the actual product or products
  associated with that reason.
- Prioritize recommendations for reasons associated with higher
  recorded waste costs, while still addressing every reason.
- Use the recorded quantities and costs when explaining why an
  action may be useful.
- Avoid repeating the same recommendation for different reasons.
- Do not assume causes that are not shown in the report.

Use these guidelines when relevant:

- Unsold: Suggest reviewing production planning and monitoring
  unsold quantities to help reduce excess production.
- Spilled: Suggest reviewing food handling, preparation, and
  serving procedures to help prevent spills.
- Expired: Suggest monitoring product expiry dates and stock rotation.
- Damaged: Suggest checking product handling, storage, and packaging.
- Overproduction: Suggest reviewing production quantities against
  recorded waste levels.

These are general suggestions only. Adapt them to the actual
products and recorded information.

If a reason does not match the examples above, create a practical
recommendation based on the recorded reason without inventing
its cause.

If a reason is associated with multiple products, mention the
relevant products when possible.

If there are no waste records, state that there is insufficient
data to provide specific recommendations.

HOLIDAY CONTEXT:

The following information contains official Philippine public holidays:

{holidayData}

Analyze whether recorded waste dates coincide with or occur near
these holidays.

IMPORTANT HOLIDAY RULES:

- Identify exact holiday date matches.
- You may also identify records occurring within three calendar
  days before or after a listed holiday.
- Clearly distinguish an exact date match from a nearby date.
- Identify possible seasonal relationships only when supported
  by the available records.
- Do not assume that a holiday caused increased waste.
- Do not invent holiday-related sales, customer demand, or events.
- If there is insufficient data, acknowledge this limitation.

GENERAL RULES:

- Use simple and understandable English.
- Use plain text only.
- Do not use Markdown formatting.
- Do not use asterisks or hashtags.
- Use the four numbered headings exactly as provided.
- Use short paragraphs and numbered recommendations.
- Display monetary amounts using Philippine peso (₱)
  with two decimal places.
- Do not change, calculate unnecessarily, or invent numerical values.
- Do not claim that a pattern is recurring based on limited records.
- Clearly distinguish recorded facts from suggestions.
- Do not omit any product, category, or distinct waste reason
  that is relevant to the analysis.
";

            var response = await _client.Models.GenerateContentAsync(
                model: "gemini-3.5-flash-lite",
                contents: prompt
            );

            return response.Candidates?[0].Content?.Parts?[0].Text
                ?? "Gemini could not generate insights. Please try again.";
        }
    }
}

