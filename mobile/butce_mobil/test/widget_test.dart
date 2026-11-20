import 'package:flutter_test/flutter_test.dart';
import 'package:butce_mobil/main.dart';

void main() {
  testWidgets('Giriş ekranı açılış testi', (WidgetTester tester) async {
    await tester.pumpWidget(const AkilliButceApp());

    expect(find.text('Akıllı Bütçe Giriş'), findsOneWidget);
    expect(find.text('Giriş Yap'), findsOneWidget);
  });
}
