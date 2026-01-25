using Portiforce.SimpleAssetAssistant.Core.Assets.Enums;
using Portiforce.SimpleAssetAssistant.Core.Assets.Models;

namespace Portiforce.SimpleAssetAssistant.Infrastructure.EF.DataPopulation.Seeders;

public static class PlatformDataSeeder
{
	public static List<Platform> GetPlatforms()
	{
		var platforms = new List<Platform>();

		// ---------------------------------------------------------
		// 1. CRYPTO EXCHANGES (Top 25)
		// ---------------------------------------------------------
		var exchanges = new List<(string Code, string Name)>
		{
			("BINANCE", "Binance"),
			("COINBASE", "Coinbase"),
			("KRAKEN", "Kraken"),
			("KUCOIN", "KuCoin"),
			("BYBIT", "Bybit"),
			("OKX", "OKX"),
			("GATEIO", "Gate.io"),
			("BITFINEX", "Bitfinex"),
			("BITSTAMP", "Bitstamp"),
			("HTX", "HTX (Huobi)"),
			("GEMINI", "Gemini"),
			("CRYPTOCOM", "Crypto.com Exchange"),
			("MEXC", "MEXC Global"),
			("BITGET", "Bitget"),
			("LBANK", "LBank"),
			("PHEMEX", "Phemex"),
			("COINEX", "CoinEx"),
			("POLONIEX", "Poloniex"),
			("DERIBIT", "Deribit"),
			("BITMART", "BitMart"),
			("UPBIT", "Upbit"),
			("BITHUMB", "Bithumb"),
			("COINCHECK", "Coincheck"),
			("BITFLYER", "bitFlyer"),
			("ASCENDEX", "AscendEX"),
			("WHITEBIT", "WhiteBit"),
			("CEXIO", "Cex.IO"),
			("CEXPLUS", "Cex Plus")
		};

		foreach (var ex in exchanges)
		{
			platforms.Add(Platform.Create(
				ex.Name,
				ex.Code,
				PlatformKind.Exchange,
				PlatformState.Active
			));
		}

		// ---------------------------------------------------------
		// 2. WALLETS (Top 10)
		// ---------------------------------------------------------
		var wallets = new List<(string Code, string Name)>
		{
			("METAMASK", "MetaMask"),
			("TRUST", "Trust Wallet"),
			("LEDGER", "Ledger"),
			("TREZOR", "Trezor"),
			("EXODUS", "Exodus"),
			("PHANTOM", "Phantom"),
			("CBWALLET", "Coinbase Wallet"),
			("ATOMIC", "Atomic Wallet"),
			("MEW", "MyEtherWallet"),
			("ELECTRUM", "Electrum")
		};

		foreach (var w in wallets)
		{
			platforms.Add(Platform.Create(
				w.Name,
				w.Code,
				PlatformKind.Wallet,
				PlatformState.Active
			));
		}

		// ---------------------------------------------------------
		// 3. TRADING PLATFORMS (Top 5 Stocks/Mixed)
		// ---------------------------------------------------------
		var brokers = new List<(string Code, string Name)>
		{
			("ROBINHOOD", "Robinhood"),
			("T212", "Trading 212"),
			("IBKR", "Interactive Brokers"),
			("ETORO", "eToro"),
			("FIDELITY", "Fidelity")
		};

		foreach (var b in brokers)
		{
			platforms.Add(
				Platform.Create(
					b.Name,
					b.Code,
					PlatformKind.Broker, 
					PlatformState.Active
			));
		}

		return platforms;
	}
}
