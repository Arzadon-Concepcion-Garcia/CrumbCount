
using Google.GenAI;

namespace CrumbCount2.Services
{
    public class GeminiService
    {
        private readonly Client _client;

        public GeminiService()
        {
            _client = new Client();
        }

        public async Task<string> GenerateWasteInsights(string reportData)
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

2. HIGH WASTE PRODUCTS AND CATEGORIES

Identify the products and categories with the highest recorded waste costs.
If there is only one product or category, state that clearly.

3. COMMON REASONS FOR WASTE

Identify the recorded reasons for waste.
Explain which reason accounts for the highest quantity and cost.

4. RECOMMENDATIONS

Provide practical suggestions based on the recorded waste reasons.
Make recommendations relevant to the actual products and waste data.
If a recorded reason seems unusual for a product, suggest checking
whether the waste record was entered correctly.
Do not assume causes that are not shown in the report.

IMPORTANT RULES:

- Use simple and understandable English.
- Use plain text only.
- Do not use Markdown formatting.
- Do not use asterisks or hashtags.
- Use numbered headings.
- Use short paragraphs and numbered recommendations.
- Display all monetary amounts using Philippine peso (₱)
  with two decimal places.
- Do not change or invent numerical values.
- Do not claim that a pattern is recurring based on limited records.
- Base observations only on the supplied data.
- Clearly distinguish recorded facts from suggestions.
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
