using System.Net.Http.Json;
using ZXing.Net.Maui;

namespace Stamps.Pages;

public partial class ScannerPage : ContentPage
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl = "https://byssal-janene-lyingly.ngrok-free.dev";
    
    private int _cardTypeId;
    private string _cardTypeName = "";
    private int _stampsToAdd = 1;
    private int _storeId;
    private string _storeOwnerId = "";
    private bool _isProcessing = false;

    public ScannerPage(int cardTypeId, string cardTypeName, int stampsToAdd, int storeId, string storeOwnerId)
    {
        InitializeComponent();
        
        _cardTypeId = cardTypeId;
        _cardTypeName = cardTypeName;
        _stampsToAdd = stampsToAdd;
        _storeId = storeId;
        _storeOwnerId = storeOwnerId;
        
        _httpClient = new HttpClient();
        
        cardTypeLabel.Text = _cardTypeName;
        stampsLabel.Text = _stampsToAdd.ToString();
        
        // Configure barcode reader options
        barcodeReader.Options = new ZXing.Net.Maui.BarcodeReaderOptions
        {
            Formats = ZXing.Net.Maui.BarcodeFormat.QrCode,
            AutoRotate = true,
            Multiple = false
        };
    }

    private async void OnBarcodesDetected(object? sender, BarcodeDetectionEventArgs e)
    {
        if (_isProcessing) return;
        
        var result = e.Results?.FirstOrDefault();
        if (result == null) return;
        
        _isProcessing = true;
        barcodeReader.IsDetecting = false;
        
        // Vibrate for feedback
        try { Vibration.Vibrate(TimeSpan.FromMilliseconds(100)); } catch { }
        
        var qrData = result.Value;
        
        // Parse QR data - expected format: "STAMPS:userId:userName" or just userId
        var customerId = qrData;
        if (qrData.StartsWith("STAMPS:"))
        {
            var parts = qrData.Split(':');
            if (parts.Length >= 2)
            {
                customerId = parts[1];
            }
        }
        
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            await ProcessStamp(customerId);
        });
    }

    private async Task ProcessStamp(string customerId)
    {
        try
        {
            var request = new
            {
                customerId = customerId,
                storeOwnerId = _storeOwnerId,
                storeId = _storeId,
                cardTypeId = _cardTypeId,
                stampsToAdd = _stampsToAdd,
                requiredStamps = 10
            };

            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/stampcards/add-stamp", request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<StampResult>();
                
                var message = result?.IsComplete == true 
                    ? $"🎉 Carta Completata!\n{result.CurrentStamps}/{result.RequiredStamps} timbri"
                    : $"Timbro aggiunto!\n{result?.CurrentStamps ?? 0}/{result?.RequiredStamps ?? 10} timbri";
                
                bool scanAgain = await DisplayAlert("Successo ✓", message, "Scansiona Altro", "Chiudi");
                
                if (scanAgain)
                {
                    _isProcessing = false;
                    barcodeReader.IsDetecting = true;
                }
                else
                {
                    await Navigation.PopModalAsync();
                }
            }
            else
            {
                bool retry = await DisplayAlert("Errore", "Cliente non trovato o QR code non valido", "Riprova", "Chiudi");
                
                if (retry)
                {
                    _isProcessing = false;
                    barcodeReader.IsDetecting = true;
                }
                else
                {
                    await Navigation.PopModalAsync();
                }
            }
        }
        catch (Exception ex)
        {
            bool retry = await DisplayAlert("Errore", $"Errore di connessione: {ex.Message}", "Riprova", "Chiudi");
            
            if (retry)
            {
                _isProcessing = false;
                barcodeReader.IsDetecting = true;
            }
            else
            {
                await Navigation.PopModalAsync();
            }
        }
    }

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        barcodeReader.IsDetecting = false;
    }

    private class StampResult
    {
        public bool Success { get; set; }
        public int CurrentStamps { get; set; }
        public int RequiredStamps { get; set; }
        public bool IsComplete { get; set; }
        public string Message { get; set; } = "";
    }
}

