import 'dart:convert';
import 'dart:io';

import 'package:http/http.dart' as http;

import '../models/harcama.dart';

class ApiServisi {
  // iOS Simülatör için 127.0.0.1, Android Emülatör için 10.0.2.2
  static String get baseUrl => Platform.isAndroid
      ? 'http://10.0.2.2:5044/api'
      : 'http://127.0.0.1:5044/api';

  Future<dynamic> girisYap(String eposta, String sifre) async {
    final yanit = await http.post(
      Uri.parse('$baseUrl/auth/giris'),
      headers: {'Content-Type': 'application/json'},
      body: jsonEncode({'eposta': eposta, 'sifre': sifre}),
    );
    if (yanit.statusCode == 200) {
      return jsonDecode(yanit.body);
    }
    return null;
  }

  Future<List<Harcama>> harcamalariGetir() async {
    final yanit = await http.get(Uri.parse('$baseUrl/harcamalar'));
    if (yanit.statusCode == 200) {
      final List<dynamic> veri = jsonDecode(yanit.body);
      return veri.map((e) => Harcama.fromJson(e)).toList();
    }
    throw Exception('Harcamalar yüklenemedi (${yanit.statusCode})');
  }

  Future<bool> harcamaEkle(String baslik, double tutar, int kategoriId) async {
    final yanit = await http.post(
      Uri.parse('$baseUrl/harcamalar'),
      headers: {'Content-Type': 'application/json'},
      body: jsonEncode({
        'baslik': baslik,
        'tutar': tutar,
        'kategoriId': kategoriId,
        'tarih': DateTime.now().toIso8601String(),
        'exifTemizlendiMi': true,
      }),
    );
    return yanit.statusCode == 201;
  }

  Future<bool> harcamaSil(int id) async {
    final yanit = await http.delete(Uri.parse('$baseUrl/harcamalar/$id'));
    return yanit.statusCode == 204;
  }
}
