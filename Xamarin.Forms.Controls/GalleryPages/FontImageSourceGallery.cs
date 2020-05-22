﻿using System.Collections.Generic;
using Xamarin.Forms.Controls.Issues;

namespace Xamarin.Forms.Controls
{
	public class FontImageSourceGallery : ContentPage
	{
		public FontImageSourceGallery()
		{
			var grid = new Grid
			{
				HeightRequest = 1696,
				WidthRequest = 320,
				BackgroundColor = Color.Black
			};
			grid.AddRowDef(count: 53);
			grid.AddColumnDef(count: 10);

			var fontFamily = "";
			switch (Device.RuntimePlatform)
			{
				case Device.macOS:
				case Device.iOS:
					fontFamily = "Ionicons";
					break;
				case Device.UWP:
					fontFamily = "Assets/Fonts/ionicons.ttf#ionicons";
					break;
				case Device.WPF:
				case Device.GTK:
					fontFamily = "Assets/ionicons.ttf#ionicons";
					break;
				case Device.Android:
				default:
					fontFamily = "fonts/ionicons.ttf#";
					break;
			}

			grid.Children.Add(new ImageButton
			{
				Source = new FontImageSource
				{
					Color = Color.White,
					Glyph = Ionicons[Ionicons.Length - 1].ToString(),
					FontFamily = fontFamily,
					Size = 20
				},
			});

			var i = 1;

			grid.Children.Add(new Image
			{
				Source = new LayeredFontImageSource
				{
					Color = Color.White,
					Layers = new List<FontImageSource> {
						new FontImageSource
						{
							Glyph = '\uf2d1'.ToString(),
							FontFamily = fontFamily,
							Size = 20
						},
						new FontImageSource
						{
							Glyph = '\uf100'.ToString(),
							FontFamily = fontFamily,
							Color = Color.Yellow,
							Size = 20
						},
					},
				},
				BackgroundColor = Color.Black,
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.Center,
			}, i % 10, i / 10);
			i++;

			grid.Children.Add(new Image
			{
				Source = new LayeredFontImageSource
				{
					Size = 30,
					Color = Color.White,
					Layers = new List<FontImageSource> {
						new FontImageSource
						{
							Glyph = '\uf2d1'.ToString(),
							FontFamily = fontFamily,
							Size = 20
						},
						new FontImageSource
						{
							Glyph = '\uf100'.ToString(),
							FontFamily = fontFamily,
							Color = Color.Yellow,
							Size = 20
						},
					},
				},
				BackgroundColor = Color.Black,
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.Center,
			}, i % 10, i / 10);
			i++;

			grid.Children.Add(new Image
			{
				Source = new LayeredFontImageSource
				{
					Size = 20,
					Color = Color.White,
					Layers = new List<FontImageSource> {
						new FontImageSource
						{
							Glyph = '\uf2d1'.ToString(),
							FontFamily = fontFamily,
							Size = 30
						},
						new FontImageSource
						{
							Glyph = '\uf100'.ToString(),
							FontFamily = fontFamily,
							Color = Color.Yellow,
							Size = 30
						},
					},
				},
				BackgroundColor = Color.Black,
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.Center,
			}, i % 10, i / 10);
			i++;

			grid.Children.Add(new Image
			{
				Source = new LayeredFontImageSource
				{
					Size = 30,
					Color = Color.White,
					Layers = new List<FontImageSource> {
						new FontImageSource
						{
							Glyph = '\uf2d1'.ToString(),
							FontFamily = fontFamily,
							Size = 30
						},
						new FontImageSource
						{
							Glyph = '\uf100'.ToString(),
							FontFamily = fontFamily,
							Color = Color.Orange,
							Size = 20
						},
					},
				},
				BackgroundColor = Color.Black,
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.Center,
			}, i % 10, i / 10);
			i++;

			grid.Children.Add(new Image
			{
				Source = new LayeredFontImageSource
				{
					FontFamily = fontFamily,
					Size = 20,
					Layers = new List<FontImageSource> {
						new FontImageSource
						{
							Glyph = '\uf23e'.ToString(),
							Color = Color.White,
						},
						new FontImageSource
						{
							Glyph = '\uf23f'.ToString(),
							Color = Color.CornflowerBlue,
						},
					},
				},
				BackgroundColor = Color.Black,
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.Center,
			}, i % 10, i / 10);
			i++;

			grid.Children.Add(new Image
			{
				Source = new LayeredFontImageSource
				{
					FontFamily = fontFamily,
					Size = 20,
					Layers = new List<FontImageSource> {
						new FontImageSource
						{
							Glyph = '\uf24c'.ToString(),
							Color = Color.White,
						},
						new FontImageSource
						{
							Glyph = '\uf24d'.ToString(),
							Color = Color.Red,
						},
					},
				},
				BackgroundColor = Color.Black,
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.Center,
			}, i % 10, i / 10);
			i++;

			foreach (char c in Ionicons)
			{
				grid.Children.Add(new Image
				{
					Source = new FontImageSource
					{
						Color = Color.White,
						Glyph = c.ToString(),
						FontFamily = fontFamily,
						Size = 20
					},
					BackgroundColor = Color.Black,
					VerticalOptions = LayoutOptions.Center,
					HorizontalOptions = LayoutOptions.Center,
				}, i % 10, i / 10);
				i++;
			}

			Content = new ScrollView
			{
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Content = grid
			};

			var tb1 = new ToolbarItem()
			{
				Text = "tb1",
				IconImageSource =  new FontImageSource()
				{
					FontFamily = fontFamily, Glyph = '\uf101'.ToString()
				}
			};
			var tb2 = new ToolbarItem
			{
				Text = "tb2 red",
				IconImageSource = new FontImageSource()
				{
					FontFamily = fontFamily, Glyph = '\uf101'.ToString(), Color = Color.Red
				},
			};
			var tb3 = new ToolbarItem
			{
				Text = "tb3 yellow",
				IconImageSource = new FontImageSource()
				{
					FontFamily = fontFamily, Glyph = '\uf2c7'.ToString(), Color = Color.Yellow
				},
			};
			ToolbarItems.Add(tb1);
			ToolbarItems.Add(tb2);
			ToolbarItems.Add(tb3);
		}

		static readonly char[] Ionicons = new char[] {
				'\uf101',
				'\uf100',
				'\uf2c7',
				'\uf2c6',
				'\uf2c8',
				'\uf2c9',
				'\uf2ca',
				'\uf2cb',
				'\uf2cc',
				'\uf2cd',
				'\uf2ce',
				'\uf2cf',
				'\uf2d0',
				'\uf2d1',
				'\uf2d2',
				'\uf2d3',
				'\uf2d4',
				'\uf2d5',
				'\uf2d6',
				'\uf2d7',
				'\uf2d8',
				'\uf2d9',
				'\uf2da',
				'\uf2db',
				'\uf2dc',
				'\uf2dd',
				'\uf2de',
				'\uf2df',
				'\uf2e0',
				'\uf2e1',
				'\uf2e2',
				'\uf2e3',
				'\uf2e4',
				'\uf2e5',
				'\uf2e6',
				'\uf2e7',
				'\uf2e8',
				'\uf2e9',
				'\uf2ea',
				'\uf2eb',
				'\uf2ec',
				'\uf2ed',
				'\uf2ee',
				'\uf2ef',
				'\uf2f0',
				'\uf2f1',
				'\uf2f2',
				'\uf2f3',
				'\uf2f4',
				'\uf2f5',
				'\uf2f6',
				'\uf2f7',
				'\uf2f8',
				'\uf2fa',
				'\uf2f9',
				'\uf2fb',
				'\uf2fc',
				'\uf2fd',
				'\uf2fe',
				'\uf2ff',
				'\uf300',
				'\uf301',
				'\uf302',
				'\uf303',
				'\uf304',
				'\uf305',
				'\uf102',
				'\uf103',
				'\uf104',
				'\uf105',
				'\uf25e',
				'\uf25f',
				'\uf260',
				'\uf261',
				'\uf262',
				'\uf106',
				'\uf107',
				'\uf108',
				'\uf263',
				'\uf264',
				'\uf265',
				'\uf266',
				'\uf109',
				'\uf10a',
				'\uf10b',
				'\uf267',
				'\uf268',
				'\uf10c',
				'\uf10d',
				'\uf10e',
				'\uf10f',
				'\uf110',
				'\uf111',
				'\uf112',
				'\uf113',
				'\uf114',
				'\uf115',
				'\uf269',
				'\uf26a',
				'\uf116',
				'\uf26b',
				'\uf26c',
				'\uf2be',
				'\uf26d',
				'\uf117',
				'\uf118',
				'\uf119',
				'\uf11b',
				'\uf11a',
				'\uf11c',
				'\uf11e',
				'\uf11d',
				'\uf11f',
				'\uf122',
				'\uf120',
				'\uf121',
				'\uf123',
				'\uf124',
				'\uf125',
				'\uf126',
				'\uf127',
				'\uf26e',
				'\uf12a',
				'\uf128',
				'\uf129',
				'\uf12b',
				'\uf271',
				'\uf26f',
				'\uf270',
				'\uf272',
				'\uf273',
				'\uf12c',
				'\uf274',
				'\uf275',
				'\uf12d',
				'\uf12f',
				'\uf12e',
				'\uf130',
				'\uf276',
				'\uf2bf',
				'\uf277',
				'\uf131',
				'\uf132',
				'\uf133',
				'\uf306',
				'\uf278',
				'\uf134',
				'\uf135',
				'\uf279',
				'\uf137',
				'\uf136',
				'\uf138',
				'\uf139',
				'\uf27a',
				'\uf2c0',
				'\uf13a',
				'\uf13b',
				'\uf13c',
				'\uf13d',
				'\uf13e',
				'\uf13f',
				'\uf27b',
				'\uf140',
				'\uf141',
				'\uf143',
				'\uf27c',
				'\uf142',
				'\uf144',
				'\uf27d',
				'\uf146',
				'\uf145',
				'\uf147',
				'\uf148',
				'\uf14a',
				'\uf149',
				'\uf14b',
				'\uf14d',
				'\uf14c',
				'\uf14f',
				'\uf14e',
				'\uf150',
				'\uf151',
				'\uf152',
				'\uf153',
				'\uf154',
				'\uf27e',
				'\uf27f',
				'\uf280',
				'\uf281',
				'\uf155',
				'\uf157',
				'\uf156',
				'\uf159',
				'\uf158',
				'\uf15b',
				'\uf15a',
				'\uf15d',
				'\uf15c',
				'\uf15f',
				'\uf15e',
				'\uf283',
				'\uf282',
				'\uf161',
				'\uf160',
				'\uf285',
				'\uf284',
				'\uf163',
				'\uf162',
				'\uf165',
				'\uf164',
				'\uf167',
				'\uf166',
				'\uf169',
				'\uf168',
				'\uf16b',
				'\uf16a',
				'\uf16e',
				'\uf16c',
				'\uf16d',
				'\uf16f',
				'\uf170',
				'\uf172',
				'\uf171',
				'\uf2bc',
				'\uf2bd',
				'\uf2bb',
				'\uf178',
				'\uf174',
				'\uf173',
				'\uf175',
				'\uf177',
				'\uf176',
				'\uf17a',
				'\uf308',
				'\uf307',
				'\uf179',
				'\uf17c',
				'\uf17b',
				'\uf17e',
				'\uf17d',
				'\uf180',
				'\uf17f',
				'\uf182',
				'\uf181',
				'\uf184',
				'\uf183',
				'\uf185',
				'\uf187',
				'\uf186',
				'\uf189',
				'\uf188',
				'\uf18b',
				'\uf18a',
				'\uf18d',
				'\uf18c',
				'\uf18f',
				'\uf18e',
				'\uf191',
				'\uf190',
				'\uf193',
				'\uf192',
				'\uf195',
				'\uf194',
				'\uf197',
				'\uf196',
				'\uf199',
				'\uf198',
				'\uf19c',
				'\uf19a',
				'\uf19b',
				'\uf19e',
				'\uf19d',
				'\uf1a1',
				'\uf19f',
				'\uf1a0',
				'\uf1a2',
				'\uf1a4',
				'\uf1a3',
				'\uf287',
				'\uf286',
				'\uf1a6',
				'\uf1a5',
				'\uf1a8',
				'\uf1a7',
				'\uf289',
				'\uf288',
				'\uf1ab',
				'\uf1a9',
				'\uf1aa',
				'\uf1ae',
				'\uf1ac',
				'\uf1ad',
				'\uf1b0',
				'\uf1af',
				'\uf1b2',
				'\uf1b1',
				'\uf1b4',
				'\uf1b3',
				'\uf1b5',
				'\uf1b6',
				'\uf1b8',
				'\uf1b7',
				'\uf1ba',
				'\uf1b9',
				'\uf1bc',
				'\uf1bb',
				'\uf1be',
				'\uf1bd',
				'\uf1c0',
				'\uf1bf',
				'\uf1c2',
				'\uf1c1',
				'\uf1c4',
				'\uf1c3',
				'\uf1c6',
				'\uf1c5',
				'\uf28b',
				'\uf28a',
				'\uf1c8',
				'\uf1c7',
				'\uf1cb',
				'\uf1c9',
				'\uf1ca',
				'\uf28d',
				'\uf28c',
				'\uf1cd',
				'\uf1cc',
				'\uf1cf',
				'\uf1ce',
				'\uf1d1',
				'\uf1d0',
				'\uf1d3',
				'\uf1d2',
				'\uf1d6',
				'\uf1d4',
				'\uf1d5',
				'\uf28e',
				'\uf1d8',
				'\uf1d7',
				'\uf1da',
				'\uf1d9',
				'\uf1dc',
				'\uf1db',
				'\uf1de',
				'\uf1dd',
				'\uf309',
				'\uf290',
				'\uf28f',
				'\uf1e0',
				'\uf1df',
				'\uf1e2',
				'\uf1e1',
				'\uf1e4',
				'\uf1e3',
				'\uf1e6',
				'\uf1e5',
				'\uf1e8',
				'\uf1e7',
				'\uf292',
				'\uf291',
				'\uf1ea',
				'\uf1e9',
				'\uf1ec',
				'\uf1eb',
				'\uf1ee',
				'\uf1ed',
				'\uf1f0',
				'\uf1ef',
				'\uf1f2',
				'\uf1f1',
				'\uf1f4',
				'\uf1f3',
				'\uf1f5',
				'\uf1f6',
				'\uf294',
				'\uf293',
				'\uf1f8',
				'\uf1f7',
				'\uf1f9',
				'\uf1fa',
				'\uf1fb',
				'\uf295',
				'\uf296',
				'\uf297',
				'\uf1fc',
				'\uf1fd',
				'\uf298',
				'\uf299',
				'\uf1fe',
				'\uf29a',
				'\uf29b',
				'\uf29c',
				'\uf29d',
				'\uf1ff',
				'\uf200',
				'\uf29e',
				'\uf29f',
				'\uf201',
				'\uf2a0',
				'\uf2a1',
				'\uf202',
				'\uf203',
				'\uf2a2',
				'\uf204',
				'\uf205',
				'\uf206',
				'\uf209',
				'\uf207',
				'\uf208',
				'\uf2c1',
				'\uf20a',
				'\uf20b',
				'\uf20c',
				'\uf20e',
				'\uf20d',
				'\uf2a3',
				'\uf2c2',
				'\uf2a4',
				'\uf2c3',
				'\uf20f',
				'\uf210',
				'\uf213',
				'\uf211',
				'\uf212',
				'\uf2a5',
				'\uf2a6',
				'\uf2a7',
				'\uf2a8',
				'\uf214',
				'\uf215',
				'\uf30a',
				'\uf218',
				'\uf216',
				'\uf217',
				'\uf219',
				'\uf2a9',
				'\uf2aa',
				'\uf2ab',
				'\uf21a',
				'\uf2ac',
				'\uf21b',
				'\uf21c',
				'\uf21e',
				'\uf21d',
				'\uf21f',
				'\uf2ad',
				'\uf220',
				'\uf221',
				'\uf222',
				'\uf223',
				'\uf225',
				'\uf224',
				'\uf227',
				'\uf226',
				'\uf2af',
				'\uf2ae',
				'\uf229',
				'\uf228',
				'\uf22b',
				'\uf22a',
				'\uf22d',
				'\uf22c',
				'\uf22f',
				'\uf22e',
				'\uf231',
				'\uf230',
				'\uf2c4',
				'\uf233',
				'\uf232',
				'\uf235',
				'\uf234',
				'\uf237',
				'\uf236',
				'\uf239',
				'\uf238',
				'\uf2b1',
				'\uf2b0',
				'\uf23b',
				'\uf23a',
				'\uf23d',
				'\uf23c',
				'\uf23f',
				'\uf23e',
				'\uf241',
				'\uf240',
				'\uf2c5',
				'\uf243',
				'\uf242',
				'\uf245',
				'\uf244',
				'\uf247',
				'\uf246',
				'\uf249',
				'\uf248',
				'\uf24b',
				'\uf24a',
				'\uf24d',
				'\uf24c',
				'\uf2b2',
				'\uf2b3',
				'\uf2b4',
				'\uf24e',
				'\uf2b5',
				'\uf30b',
				'\uf24f',
				'\uf2b6',
				'\uf250',
				'\uf251',
				'\uf252',
				'\uf253',
				'\uf2b7',
				'\uf254',
				'\uf255',
				'\uf2b8',
				'\uf256',
				'\uf257',
				'\uf258',
				'\uf259',
				'\uf25a',
				'\uf25b',
				'\uf25c',
				'\uf2b9',
				'\uf25d',
				'\uf2ba',
				'\uf30c',
			};
	}
}
