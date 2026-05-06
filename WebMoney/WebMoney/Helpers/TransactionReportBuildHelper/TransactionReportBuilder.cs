using System.Globalization;
using System.Text;
using Microsoft.Extensions.Localization;
using WebMoney.Data.Entities;

namespace WebMoney.Helpers.TransactionReportBuildHelper;

public static class TransactionReportBuilder
{
    public static byte[] BuildCsvBytes(IReadOnlyList<Transaction> transactions,
        IStringLocalizer<SharedResource> sharedLocalizer)
    {
        var separator = CultureInfo.CurrentCulture.TextInfo.ListSeparator;
        var dateHeader = sharedLocalizer["Txn_ColDate"].Value ?? "Date";
        var typeHeader = sharedLocalizer["Txn_ColType"].Value ?? "Type";
        var statusHeader = sharedLocalizer["Txn_ColStatus"].Value ?? "Status";
        var currencyHeader = sharedLocalizer["Txn_ColCurrency"].Value ?? "Currency";
        var amountHeader = sharedLocalizer["Txn_ColAmount"].Value ?? "Amount";
        var counterpartyHeader = sharedLocalizer["Txn_ColCounterparty"].Value ?? "Counterparty";

        using var stream = new MemoryStream();
        using (var writer = new StreamWriter(stream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true),
                   leaveOpen: true))
        {
            writer.Write(FormatCsvField(dateHeader, separator));
            writer.Write(separator);
            writer.Write(FormatCsvField(typeHeader, separator));
            writer.Write(separator);
            writer.Write(FormatCsvField(statusHeader, separator));
            writer.Write(separator);
            writer.Write(FormatCsvField(currencyHeader, separator));
            writer.Write(separator);
            writer.Write(FormatCsvField(amountHeader, separator));
            writer.Write(separator);
            writer.WriteLine(FormatCsvField(counterpartyHeader, separator));

            foreach (var transaction in transactions)
            {
                var localizedType = sharedLocalizer[$"Enum_TransactionType_{transaction.TransactionType}"].Value
                                    ?? transaction.TransactionType.ToString();
                var localizedStatus = sharedLocalizer[$"Enum_TransactionStatus_{transaction.TransactionStatus}"].Value
                                      ?? transaction.TransactionStatus.ToString();
                var localizedCurrency = sharedLocalizer[$"Enum_CurrencyCode_{transaction.Card.CurrencyCode}"].Value
                                        ?? transaction.Card.CurrencyCode.ToString();

                writer.Write(FormatCsvField(transaction.CreatedAt.ToString("g", CultureInfo.CurrentCulture), separator));
                writer.Write(separator);
                writer.Write(FormatCsvField(localizedType, separator));
                writer.Write(separator);
                writer.Write(FormatCsvField(localizedStatus, separator));
                writer.Write(separator);
                writer.Write(FormatCsvField(localizedCurrency, separator));
                writer.Write(separator);
                writer.Write(FormatCsvField(transaction.Amount.ToString("N2", CultureInfo.CurrentCulture), separator));
                writer.Write(separator);
                writer.WriteLine(FormatCsvField(transaction.Counterparty.Name, separator));
            }

            writer.Flush();
        }

        return stream.ToArray();
    }

    private static string FormatCsvField(string? value, string separator)
    {
        var normalized = value ?? string.Empty;
        if (!normalized.Contains(separator) && !normalized.Contains('"') && !normalized.Contains('\n') &&
            !normalized.Contains('\r'))
        {
            return normalized;
        }

        return $"\"{normalized.Replace("\"", "\"\"")}\"";
    }
}