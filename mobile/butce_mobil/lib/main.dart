import 'package:flutter/material.dart';

import 'models/harcama.dart';
import 'services/api_servisi.dart';

void main() => runApp(const AkilliButceApp());

class AkilliButceApp extends StatelessWidget {
  const AkilliButceApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Akıllı Bütçe',
      debugShowCheckedModeBanner: false,
      theme: ThemeData(colorSchemeSeed: Colors.teal, useMaterial3: true),
      home: const GirisEkrani(),
    );
  }
}

// 1. EKRAN: Giriş ve Rol Kontrolü
class GirisEkrani extends StatefulWidget {
  const GirisEkrani({super.key});
  @override
  State<GirisEkrani> createState() => _GirisEkraniState();
}

class _GirisEkraniState extends State<GirisEkrani> {
  final _formKey = GlobalKey<FormState>();
  final _epostaCtrl = TextEditingController(text: 'admin@butce.local');
  final _sifreCtrl = TextEditingController(text: 'Yonetici2026!');
  final _api = ApiServisi();
  bool _yukleniyor = false;

  @override
  void dispose() {
    _epostaCtrl.dispose();
    _sifreCtrl.dispose();
    super.dispose();
  }

  Future<void> _giris() async {
    if (!_formKey.currentState!.validate()) return;
    setState(() => _yukleniyor = true);

    try {
      final sonuc = await _api.girisYap(
        _epostaCtrl.text.trim(),
        _sifreCtrl.text,
      );
      if (!mounted) return;
      setState(() => _yukleniyor = false);

      if (sonuc != null) {
        Navigator.pushReplacement(
          context,
          MaterialPageRoute(
            builder: (_) =>
                AnaKabukEkrani(eposta: sonuc['eposta'], rol: sonuc['rol']),
          ),
        );
      } else {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('Giriş başarısız. E-posta veya şifre hatalı.'),
          ),
        );
      }
    } catch (e) {
      if (!mounted) return;
      setState(() => _yukleniyor = false);
      ScaffoldMessenger.of(context)
          .showSnackBar(SnackBar(content: Text('Sunucuya bağlanılamadı: $e')));
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Akıllı Bütçe Giriş')),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(20),
        child: Form(
          key: _formKey,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              const Icon(
                Icons.account_balance_wallet,
                size: 72,
                color: Colors.teal,
              ),
              const SizedBox(height: 16),
              TextFormField(
                controller: _epostaCtrl,
                decoration: const InputDecoration(
                  labelText: 'E-posta',
                  border: OutlineInputBorder(),
                ),
                validator: (v) => (v == null || !v.contains('@'))
                    ? 'Geçerli e-posta girin'
                    : null,
              ),
              const SizedBox(height: 12),
              TextFormField(
                controller: _sifreCtrl,
                obscureText: true,
                decoration: const InputDecoration(
                  labelText: 'Şifre',
                  border: OutlineInputBorder(),
                ),
                validator: (v) =>
                    (v == null || v.length < 6) ? 'En az 6 karakter' : null,
              ),
              const SizedBox(height: 20),
              FilledButton(
                onPressed: _yukleniyor ? null : _giris,
                child: Text(_yukleniyor ? 'Bağlanıyor...' : 'Giriş Yap'),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

// 2. EKRAN: Alt Menü Kabuğu (Özet, Harcamalar, Gizlilik ve Admin Paneli)
class AnaKabukEkrani extends StatefulWidget {
  final String eposta;
  final String rol;
  const AnaKabukEkrani({super.key, required this.eposta, required this.rol});

  @override
  State<AnaKabukEkrani> createState() => _AnaKabukEkraniState();
}

class _AnaKabukEkraniState extends State<AnaKabukEkrani> {
  int _seciliSekme = 0;

  @override
  Widget build(BuildContext context) {
    final adminMi = widget.rol == 'Admin';
    final sayfalar = <Widget>[
      const OzetEkrani(),
      const HarcamalarEkrani(),
      const GizlilikEkrani(),
      if (adminMi) YonetimEkrani(eposta: widget.eposta),
    ];

    return Scaffold(
      body: sayfalar[_seciliSekme],
      bottomNavigationBar: NavigationBar(
        selectedIndex: _seciliSekme,
        onDestinationSelected: (i) => setState(() => _seciliSekme = i),
        destinations: [
          const NavigationDestination(
            icon: Icon(Icons.dashboard),
            label: 'Özet',
          ),
          const NavigationDestination(
            icon: Icon(Icons.list_alt),
            label: 'Harcamalar',
          ),
          const NavigationDestination(
            icon: Icon(Icons.security),
            label: 'Gizlilik',
          ),
          if (adminMi)
            const NavigationDestination(
              icon: Icon(Icons.admin_panel_settings),
              label: 'Yönetim',
            ),
        ],
      ),
    );
  }
}

// 3. EKRAN: Canlı Bütçe Özeti ve Abonelik Uyarıları
class OzetEkrani extends StatelessWidget {
  const OzetEkrani({super.key});

  @override
  Widget build(BuildContext context) {
    final api = ApiServisi();
    return Scaffold(
      appBar: AppBar(title: const Text('Bütçe & Abonelik Özeti')),
      body: FutureBuilder<List<Harcama>>(
        future: api.harcamalariGetir(),
        builder: (context, snapshot) {
          final liste = snapshot.data ?? [];
          final toplam = liste.fold<double>(0, (sum, item) => sum + item.tutar);

          return ListView(
            padding: const EdgeInsets.all(16),
            children: [
              Card(
                color: Colors.teal.shade50,
                child: ListTile(
                  leading: const Icon(
                    Icons.payments,
                    color: Colors.teal,
                    size: 36,
                  ),
                  title: const Text('Toplam Kayıtlı Harcama'),
                  subtitle: Text(
                    '${toplam.toStringAsFixed(2)} TL (${liste.length} işlem)',
                    style: const TextStyle(
                      fontSize: 18,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ),
              ),
              const SizedBox(height: 12),
              const Card(
                color: Color(0xFFFFF3CD),
                child: ListTile(
                  leading: Icon(
                    Icons.warning_amber,
                    color: Colors.orange,
                    size: 36,
                  ),
                  title: Text('Yaklaşan Abonelik Uyarısı'),
                  subtitle: Text(
                    'Düzenli abonelik ödemelerinizi Web panelinden takip edebilirsiniz.',
                  ),
                ),
              ),
            ],
          );
        },
      ),
    );
  }
}

// 4. & 5. EKRAN: API'den Harcama Listesi, Kaydırarak Silme ve Ekleme
class HarcamalarEkrani extends StatefulWidget {
  const HarcamalarEkrani({super.key});
  @override
  State<HarcamalarEkrani> createState() => _HarcamalarEkraniState();
}

class _HarcamalarEkraniState extends State<HarcamalarEkrani> {
  final _api = ApiServisi();
  late Future<List<Harcama>> _listeFuture;

  @override
  void initState() {
    super.initState();
    _yenile();
  }

  void _yenile() {
    setState(() {
      _listeFuture = _api.harcamalariGetir();
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Harcamalarım'),
        actions: [
          IconButton(onPressed: _yenile, icon: const Icon(Icons.refresh)),
        ],
      ),
      floatingActionButton: FloatingActionButton(
        onPressed: () async {
          final eklendi = await Navigator.push<bool>(
            context,
            MaterialPageRoute(builder: (_) => const HarcamaEkleEkrani()),
          );
          if (eklendi == true) _yenile();
        },
        child: const Icon(Icons.add),
      ),
      body: FutureBuilder<List<Harcama>>(
        future: _listeFuture,
        builder: (context, snapshot) {
          if (snapshot.connectionState == ConnectionState.waiting) {
            return const Center(child: CircularProgressIndicator());
          }
          if (snapshot.hasError) {
            return Center(child: Text('Hata: ${snapshot.error}'));
          }
          final veriler = snapshot.data ?? [];
          if (veriler.isEmpty) {
            return const Center(
              child: Text('Henüz kayıtlı harcama yok. Sağ alttan ekleyin.'),
            );
          }
          return ListView.builder(
            itemCount: veriler.length,
            itemBuilder: (context, i) {
              final h = veriler[i];
              return Dismissible(
                key: ValueKey(h.id),
                direction: DismissDirection.endToStart,
                background: Container(
                  color: Colors.red,
                  alignment: Alignment.centerRight,
                  padding: const EdgeInsets.only(right: 20),
                  child: const Icon(Icons.delete, color: Colors.white),
                ),
                onDismissed: (_) async {
                  await _api.harcamaSil(h.id);
                  _yenile();
                },
                child: Card(
                  margin: const EdgeInsets.symmetric(
                    horizontal: 12,
                    vertical: 6,
                  ),
                  child: ListTile(
                    title: Text(h.baslik),
                    subtitle: Text(h.kategoriAdi),
                    trailing: Text(
                      '${h.tutar.toStringAsFixed(2)} TL',
                      style: const TextStyle(fontWeight: FontWeight.bold),
                    ),
                  ),
                ),
              );
            },
          );
        },
      ),
    );
  }
}

class HarcamaEkleEkrani extends StatefulWidget {
  const HarcamaEkleEkrani({super.key});
  @override
  State<HarcamaEkleEkrani> createState() => _HarcamaEkleEkraniState();
}

class _HarcamaEkleEkraniState extends State<HarcamaEkleEkrani> {
  final _formKey = GlobalKey<FormState>();
  final _baslikCtrl = TextEditingController();
  final _tutarCtrl = TextEditingController();
  int _kategoriId = 1;
  final _api = ApiServisi();

  @override
  void dispose() {
    _baslikCtrl.dispose();
    _tutarCtrl.dispose();
    super.dispose();
  }

  Future<void> _kaydet() async {
    if (!_formKey.currentState!.validate()) return;
    final tutar = double.tryParse(_tutarCtrl.text.replaceAll(',', '.')) ?? 0;
    final ok = await _api.harcamaEkle(
      _baslikCtrl.text.trim(),
      tutar,
      _kategoriId,
    );
    if (!mounted) return;
    if (ok) Navigator.pop(context, true);
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Yeni Harcama Ekle')),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(16),
        child: Form(
          key: _formKey,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              TextFormField(
                controller: _baslikCtrl,
                decoration: const InputDecoration(
                  labelText: 'Harcama Adı',
                  border: OutlineInputBorder(),
                ),
                validator: (v) =>
                    (v == null || v.trim().isEmpty) ? 'Başlık zorunlu' : null,
              ),
              const SizedBox(height: 12),
              TextFormField(
                controller: _tutarCtrl,
                keyboardType: TextInputType.number,
                decoration: const InputDecoration(
                  labelText: 'Tutar (TL)',
                  border: OutlineInputBorder(),
                ),
                validator: (v) =>
                    (double.tryParse((v ?? '').replaceAll(',', '.')) == null)
                    ? 'Geçerli sayı girin'
                    : null,
              ),
              const SizedBox(height: 12),
              DropdownButtonFormField<int>(
                value: _kategoriId,
                decoration: const InputDecoration(
                  labelText: 'Kategori',
                  border: OutlineInputBorder(),
                ),
                items: const [
                  DropdownMenuItem(value: 1, child: Text('Market & Gıda')),
                  DropdownMenuItem(value: 2, child: Text('Fatura & Kira')),
                  DropdownMenuItem(value: 3, child: Text('Ulaşım')),
                  DropdownMenuItem(value: 4, child: Text('Dijital Abonelik')),
                  DropdownMenuItem(value: 5, child: Text('Eğitim & Teknoloji')),
                ],
                onChanged: (v) => setState(() => _kategoriId = v ?? 1),
              ),
              const SizedBox(height: 20),
              FilledButton(onPressed: _kaydet, child: const Text('Kaydet')),
            ],
          ),
        ),
      ),
    );
  }
}

// 6. EKRAN: EXIF Gizlilik Koruması Bilgi Ekranı
class GizlilikEkrani extends StatelessWidget {
  const GizlilikEkrani({super.key});
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Veri Gizliliği (EXIF)')),
      body: const Padding(
        padding: EdgeInsets.all(16),
        child: Card(
          child: ListTile(
            leading: Icon(Icons.verified_user, color: Colors.teal, size: 40),
            title: Text('EXIF & GPS Temizleme Aktif'),
            subtitle: Text(
              'Sunucuya yüklenen tüm fiş fotoğrafları ImageSharp servisi ile yeniden kodlanarak GPS koordinatı ve cihaz bilgileri silinir.',
            ),
          ),
        ),
      ),
    );
  }
}

// 7. EKRAN: Yalnızca Admin Rolüne Açık Yönetim Ekranı
class YonetimEkrani extends StatelessWidget {
  final String eposta;
  const YonetimEkrani({super.key, required this.eposta});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Yönetici Kontrol Paneli')),
      body: Padding(
        padding: const EdgeInsets.all(16),
        child: Card(
          child: ListTile(
            leading: const Icon(
              Icons.admin_panel_settings,
              color: Colors.orange,
              size: 40,
            ),
            title: Text('Aktif Yönetici: $eposta'),
            subtitle: const Text(
              'Bu sekme yalnızca Admin rolüne sahip kullanıcılara görünür. Tüm API ve EXIF denetim kayıtları MSSQL üzerinde loglanmaktadır.',
            ),
          ),
        ),
      ),
    );
  }
}
