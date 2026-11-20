class Harcama {
  final int id;
  final String baslik;
  final double tutar;
  final String kategoriAdi;
  final int kategoriId;
  final bool exifTemizlendiMi;

  Harcama({
    required this.id,
    required this.baslik,
    required this.tutar,
    required this.kategoriAdi,
    required this.kategoriId,
    required this.exifTemizlendiMi,
  });

  factory Harcama.fromJson(Map<String, dynamic> json) {
    return Harcama(
      id: json['id'] ?? 0,
      baslik: json['baslik'] ?? '',
      tutar: (json['tutar'] ?? 0).toDouble(),
      kategoriAdi: json['kategoriAdi'] ?? 'Diğer',
      kategoriId: json['kategoriId'] ?? 1,
      exifTemizlendiMi: json['exifTemizlendiMi'] ?? false,
    );
  }
}
