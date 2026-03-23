# =============================================================================
# create_s3_files.ps1 — создаёт только файлы проекта S3_SpringServer
# Никакой установки пакетов, БД, сборки — только папки + файлы
# Запуск: .\create_s3_files.ps1
# =============================================================================

$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$Base = "$ScriptDir\S3_plan\S3_SpringServer"
$Src  = "$Base\src\main\java\com\example\shop"

Write-Host "[S3] Создание структуры папок..."
@(
    "$Src\config",
    "$Src\controller",
    "$Src\dto",
    "$Src\model",
    "$Src\repository",
    "$Src\service",
    "$Base\src\main\resources"
) | ForEach-Object { New-Item -ItemType Directory -Force -Path $_ | Out-Null }

Write-Host "[S3] Запись файлов..."

# --------------------------------------------------------------------------- #
# pom.xml
# --------------------------------------------------------------------------- #
Set-Content -Encoding UTF8 -Path "$Base\pom.xml" -Value @'
<?xml version="1.0" encoding="UTF-8"?>
<project xmlns="http://maven.apache.org/POM/4.0.0"
         xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
         xsi:schemaLocation="http://maven.apache.org/POM/4.0.0 https://maven.apache.org/xsd/maven-4.0.0.xsd">
    <modelVersion>4.0.0</modelVersion>

    <parent>
        <groupId>org.springframework.boot</groupId>
        <artifactId>spring-boot-starter-parent</artifactId>
        <version>3.2.3</version>
        <relativePath/>
    </parent>

    <groupId>com.example</groupId>
    <artifactId>shop</artifactId>
    <version>1.0.0</version>
    <packaging>jar</packaging>
    <name>S3_SpringServer</name>

    <properties>
        <java.version>17</java.version>
    </properties>

    <dependencies>
        <dependency>
            <groupId>org.springframework.boot</groupId>
            <artifactId>spring-boot-starter-web</artifactId>
        </dependency>
        <dependency>
            <groupId>org.springframework.boot</groupId>
            <artifactId>spring-boot-starter-data-jpa</artifactId>
        </dependency>
        <dependency>
            <groupId>org.springframework.boot</groupId>
            <artifactId>spring-boot-starter-security</artifactId>
        </dependency>
        <dependency>
            <groupId>org.springframework.boot</groupId>
            <artifactId>spring-boot-starter-validation</artifactId>
        </dependency>
        <dependency>
            <groupId>com.mysql</groupId>
            <artifactId>mysql-connector-j</artifactId>
            <scope>runtime</scope>
        </dependency>
        <dependency>
            <groupId>io.jsonwebtoken</groupId>
            <artifactId>jjwt-api</artifactId>
            <version>0.11.5</version>
        </dependency>
        <dependency>
            <groupId>io.jsonwebtoken</groupId>
            <artifactId>jjwt-impl</artifactId>
            <version>0.11.5</version>
            <scope>runtime</scope>
        </dependency>
        <dependency>
            <groupId>io.jsonwebtoken</groupId>
            <artifactId>jjwt-jackson</artifactId>
            <version>0.11.5</version>
            <scope>runtime</scope>
        </dependency>
        <dependency>
            <groupId>org.projectlombok</groupId>
            <artifactId>lombok</artifactId>
            <optional>true</optional>
        </dependency>
    </dependencies>

    <build>
        <plugins>
            <plugin>
                <groupId>org.springframework.boot</groupId>
                <artifactId>spring-boot-maven-plugin</artifactId>
                <configuration>
                    <excludes>
                        <exclude>
                            <groupId>org.projectlombok</groupId>
                            <artifactId>lombok</artifactId>
                        </exclude>
                    </excludes>
                </configuration>
            </plugin>
        </plugins>
    </build>
</project>
'@

# --------------------------------------------------------------------------- #
# application.properties
# --------------------------------------------------------------------------- #
Set-Content -Encoding UTF8 -Path "$Base\src\main\resources\application.properties" -Value @'
spring.datasource.url=jdbc:mysql://localhost:3306/ShopDB?useSSL=false&serverTimezone=UTC&allowPublicKeyRetrieval=true
spring.datasource.username=shopuser
spring.datasource.password=YourPassword123!
spring.datasource.driver-class-name=com.mysql.cj.jdbc.Driver
spring.jpa.hibernate.ddl-auto=update
spring.jpa.show-sql=true
spring.jpa.properties.hibernate.dialect=org.hibernate.dialect.MySQLDialect
server.port=8080
jwt.secret=MySecretKeyForJWTTokenGenerationMustBe256BitsLongEnough
jwt.expiration=86400000
'@

# --------------------------------------------------------------------------- #
# model
# --------------------------------------------------------------------------- #
Set-Content -Encoding UTF8 -Path "$Src\model\Role.java" -Value @'
package com.example.shop.model;

public enum Role {
    USER,
    ADMIN
}
'@

Set-Content -Encoding UTF8 -Path "$Src\model\OrderStatus.java" -Value @'
package com.example.shop.model;

public enum OrderStatus {
    PENDING,
    CONFIRMED,
    SHIPPED,
    DELIVERED,
    CANCELLED
}
'@

Set-Content -Encoding UTF8 -Path "$Src\model\User.java" -Value @'
package com.example.shop.model;

import jakarta.persistence.*;
import lombok.Data;
import java.time.LocalDateTime;

@Data
@Entity
@Table(name = "users")
public class User {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @Column(unique = true, nullable = false)
    private String username;

    @Column(unique = true, nullable = false)
    private String email;

    @Column(nullable = false)
    private String password;

    @Enumerated(EnumType.STRING)
    @Column(nullable = false)
    private Role role = Role.USER;

    @Column(nullable = false)
    private boolean active = true;

    @Column(nullable = false)
    private LocalDateTime createdAt = LocalDateTime.now();
}
'@

Set-Content -Encoding UTF8 -Path "$Src\model\Product.java" -Value @'
package com.example.shop.model;

import jakarta.persistence.*;
import lombok.Data;
import java.math.BigDecimal;
import java.time.LocalDateTime;

@Data
@Entity
@Table(name = "products")
public class Product {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @Column(nullable = false)
    private String name;

    @Column(length = 1000)
    private String description;

    @Column(nullable = false)
    private BigDecimal price;

    @Column(nullable = false)
    private int stock;

    @Column(nullable = false)
    private LocalDateTime createdAt = LocalDateTime.now();
}
'@

Set-Content -Encoding UTF8 -Path "$Src\model\Order.java" -Value @'
package com.example.shop.model;

import jakarta.persistence.*;
import lombok.Data;
import java.math.BigDecimal;
import java.time.LocalDateTime;

@Data
@Entity
@Table(name = "orders")
public class Order {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "user_id", nullable = false)
    private User user;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "product_id", nullable = false)
    private Product product;

    @Column(nullable = false)
    private int quantity;

    @Column(nullable = false)
    private BigDecimal totalPrice;

    @Enumerated(EnumType.STRING)
    @Column(nullable = false)
    private OrderStatus status = OrderStatus.PENDING;

    @Column(nullable = false)
    private LocalDateTime createdAt = LocalDateTime.now();
}
'@

# --------------------------------------------------------------------------- #
# dto
# --------------------------------------------------------------------------- #
Set-Content -Encoding UTF8 -Path "$Src\dto\LoginRequest.java" -Value @'
package com.example.shop.dto;

import lombok.Data;

@Data
public class LoginRequest {
    private String username;
    private String password;
}
'@

Set-Content -Encoding UTF8 -Path "$Src\dto\RegisterRequest.java" -Value @'
package com.example.shop.dto;

import lombok.Data;

@Data
public class RegisterRequest {
    private String username;
    private String email;
    private String password;
}
'@

Set-Content -Encoding UTF8 -Path "$Src\dto\TokenResponse.java" -Value @'
package com.example.shop.dto;

import lombok.AllArgsConstructor;
import lombok.Data;

@Data
@AllArgsConstructor
public class TokenResponse {
    private String token;
    private String username;
    private String role;
}
'@

Set-Content -Encoding UTF8 -Path "$Src\dto\ProductDto.java" -Value @'
package com.example.shop.dto;

import lombok.Data;
import java.math.BigDecimal;

@Data
public class ProductDto {
    private Long id;
    private String name;
    private String description;
    private BigDecimal price;
    private int stock;
}
'@

Set-Content -Encoding UTF8 -Path "$Src\dto\OrderDto.java" -Value @'
package com.example.shop.dto;

import lombok.Data;
import java.math.BigDecimal;

@Data
public class OrderDto {
    private Long id;
    private Long productId;
    private String productName;
    private int quantity;
    private BigDecimal totalPrice;
    private String status;
}
'@

Set-Content -Encoding UTF8 -Path "$Src\dto\CreateOrderRequest.java" -Value @'
package com.example.shop.dto;

import lombok.Data;

@Data
public class CreateOrderRequest {
    private Long productId;
    private int quantity;
}
'@

# --------------------------------------------------------------------------- #
# repository
# --------------------------------------------------------------------------- #
Set-Content -Encoding UTF8 -Path "$Src\repository\UserRepository.java" -Value @'
package com.example.shop.repository;

import com.example.shop.model.User;
import org.springframework.data.jpa.repository.JpaRepository;
import java.util.Optional;

public interface UserRepository extends JpaRepository<User, Long> {
    Optional<User> findByUsername(String username);
    boolean existsByUsername(String username);
    boolean existsByEmail(String email);
}
'@

Set-Content -Encoding UTF8 -Path "$Src\repository\ProductRepository.java" -Value @'
package com.example.shop.repository;

import com.example.shop.model.Product;
import org.springframework.data.jpa.repository.JpaRepository;
import java.util.List;

public interface ProductRepository extends JpaRepository<Product, Long> {
    List<Product> findByNameContainingIgnoreCase(String name);
}
'@

Set-Content -Encoding UTF8 -Path "$Src\repository\OrderRepository.java" -Value @'
package com.example.shop.repository;

import com.example.shop.model.Order;
import org.springframework.data.jpa.repository.JpaRepository;
import java.util.List;

public interface OrderRepository extends JpaRepository<Order, Long> {
    List<Order> findByUserId(Long userId);
}
'@

# --------------------------------------------------------------------------- #
# service
# --------------------------------------------------------------------------- #
Set-Content -Encoding UTF8 -Path "$Src\service\JwtService.java" -Value @'
package com.example.shop.service;

import io.jsonwebtoken.Claims;
import io.jsonwebtoken.Jwts;
import io.jsonwebtoken.SignatureAlgorithm;
import io.jsonwebtoken.security.Keys;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Service;

import java.security.Key;
import java.util.Date;

@Service
public class JwtService {

    @Value("${jwt.secret}")
    private String secret;

    @Value("${jwt.expiration}")
    private long expiration;

    private Key getSigningKey() {
        return Keys.hmacShaKeyFor(secret.getBytes());
    }

    public String generateToken(String username) {
        Date now = new Date();
        Date expiryDate = new Date(now.getTime() + expiration);
        return Jwts.builder()
                .setSubject(username)
                .setIssuedAt(now)
                .setExpiration(expiryDate)
                .signWith(getSigningKey(), SignatureAlgorithm.HS256)
                .compact();
    }

    public String getUsernameFromToken(String token) {
        Claims claims = Jwts.parserBuilder()
                .setSigningKey(getSigningKey())
                .build()
                .parseClaimsJws(token)
                .getBody();
        return claims.getSubject();
    }

    public boolean isTokenValid(String token) {
        try {
            Jwts.parserBuilder().setSigningKey(getSigningKey()).build().parseClaimsJws(token);
            return true;
        } catch (Exception e) {
            return false;
        }
    }
}
'@

Set-Content -Encoding UTF8 -Path "$Src\service\AuthService.java" -Value @'
package com.example.shop.service;

import com.example.shop.dto.LoginRequest;
import com.example.shop.dto.RegisterRequest;
import com.example.shop.dto.TokenResponse;
import com.example.shop.model.User;
import com.example.shop.repository.UserRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.stereotype.Service;
import org.springframework.web.server.ResponseStatusException;
import org.springframework.http.HttpStatus;

@Service
@RequiredArgsConstructor
public class AuthService {

    private final UserRepository userRepository;
    private final PasswordEncoder passwordEncoder;
    private final JwtService jwtService;

    public TokenResponse register(RegisterRequest request) {
        if (userRepository.existsByUsername(request.getUsername()))
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "Username already taken");
        if (userRepository.existsByEmail(request.getEmail()))
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "Email already registered");
        User user = new User();
        user.setUsername(request.getUsername());
        user.setEmail(request.getEmail());
        user.setPassword(passwordEncoder.encode(request.getPassword()));
        userRepository.save(user);
        String token = jwtService.generateToken(user.getUsername());
        return new TokenResponse(token, user.getUsername(), user.getRole().name());
    }

    public TokenResponse login(LoginRequest request) {
        User user = userRepository.findByUsername(request.getUsername())
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.UNAUTHORIZED, "Invalid credentials"));
        if (!user.isActive())
            throw new ResponseStatusException(HttpStatus.UNAUTHORIZED, "Account is disabled");
        if (!passwordEncoder.matches(request.getPassword(), user.getPassword()))
            throw new ResponseStatusException(HttpStatus.UNAUTHORIZED, "Invalid credentials");
        String token = jwtService.generateToken(user.getUsername());
        return new TokenResponse(token, user.getUsername(), user.getRole().name());
    }
}
'@

Set-Content -Encoding UTF8 -Path "$Src\service\ProductService.java" -Value @'
package com.example.shop.service;

import com.example.shop.dto.ProductDto;
import com.example.shop.model.Product;
import com.example.shop.repository.ProductRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.http.HttpStatus;
import org.springframework.stereotype.Service;
import org.springframework.web.server.ResponseStatusException;

import java.time.LocalDateTime;
import java.util.List;
import java.util.stream.Collectors;

@Service
@RequiredArgsConstructor
public class ProductService {

    private final ProductRepository productRepository;

    public List<ProductDto> getAll() {
        return productRepository.findAll().stream().map(this::toDto).collect(Collectors.toList());
    }

    public ProductDto getById(Long id) {
        return toDto(productRepository.findById(id)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "Product not found")));
    }

    public ProductDto create(ProductDto dto) {
        Product p = new Product();
        p.setName(dto.getName());
        p.setDescription(dto.getDescription());
        p.setPrice(dto.getPrice());
        p.setStock(dto.getStock());
        p.setCreatedAt(LocalDateTime.now());
        return toDto(productRepository.save(p));
    }

    public ProductDto update(Long id, ProductDto dto) {
        Product p = productRepository.findById(id)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "Product not found"));
        p.setName(dto.getName());
        p.setDescription(dto.getDescription());
        p.setPrice(dto.getPrice());
        p.setStock(dto.getStock());
        return toDto(productRepository.save(p));
    }

    public void delete(Long id) {
        if (!productRepository.existsById(id))
            throw new ResponseStatusException(HttpStatus.NOT_FOUND, "Product not found");
        productRepository.deleteById(id);
    }

    private ProductDto toDto(Product p) {
        ProductDto dto = new ProductDto();
        dto.setId(p.getId());
        dto.setName(p.getName());
        dto.setDescription(p.getDescription());
        dto.setPrice(p.getPrice());
        dto.setStock(p.getStock());
        return dto;
    }
}
'@

Set-Content -Encoding UTF8 -Path "$Src\service\OrderService.java" -Value @'
package com.example.shop.service;

import com.example.shop.dto.CreateOrderRequest;
import com.example.shop.dto.OrderDto;
import com.example.shop.model.Order;
import com.example.shop.model.Product;
import com.example.shop.model.User;
import com.example.shop.repository.OrderRepository;
import com.example.shop.repository.ProductRepository;
import com.example.shop.repository.UserRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.http.HttpStatus;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.web.server.ResponseStatusException;

import java.math.BigDecimal;
import java.time.LocalDateTime;
import java.util.List;
import java.util.stream.Collectors;

@Service
@RequiredArgsConstructor
public class OrderService {

    private final OrderRepository orderRepository;
    private final UserRepository userRepository;
    private final ProductRepository productRepository;

    public List<OrderDto> getOrdersForUser(String username) {
        User user = userRepository.findByUsername(username)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "User not found"));
        return orderRepository.findByUserId(user.getId()).stream().map(this::toDto).collect(Collectors.toList());
    }

    @Transactional
    public OrderDto createOrder(String username, CreateOrderRequest request) {
        User user = userRepository.findByUsername(username)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "User not found"));
        Product product = productRepository.findById(request.getProductId())
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "Product not found"));
        if (product.getStock() < request.getQuantity())
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "Not enough stock");
        product.setStock(product.getStock() - request.getQuantity());
        productRepository.save(product);
        BigDecimal totalPrice = product.getPrice().multiply(BigDecimal.valueOf(request.getQuantity()));
        Order order = new Order();
        order.setUser(user);
        order.setProduct(product);
        order.setQuantity(request.getQuantity());
        order.setTotalPrice(totalPrice);
        order.setCreatedAt(LocalDateTime.now());
        return toDto(orderRepository.save(order));
    }

    public void deleteOrder(Long id) {
        if (!orderRepository.existsById(id))
            throw new ResponseStatusException(HttpStatus.NOT_FOUND, "Order not found");
        orderRepository.deleteById(id);
    }

    private OrderDto toDto(Order o) {
        OrderDto dto = new OrderDto();
        dto.setId(o.getId());
        dto.setProductId(o.getProduct().getId());
        dto.setProductName(o.getProduct().getName());
        dto.setQuantity(o.getQuantity());
        dto.setTotalPrice(o.getTotalPrice());
        dto.setStatus(o.getStatus().name());
        return dto;
    }
}
'@

# --------------------------------------------------------------------------- #
# config
# --------------------------------------------------------------------------- #
Set-Content -Encoding UTF8 -Path "$Src\config\JwtFilter.java" -Value @'
package com.example.shop.config;

import com.example.shop.service.JwtService;
import jakarta.servlet.FilterChain;
import jakarta.servlet.ServletException;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import lombok.RequiredArgsConstructor;
import org.springframework.security.authentication.UsernamePasswordAuthenticationToken;
import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.security.core.userdetails.UserDetails;
import org.springframework.security.core.userdetails.UserDetailsService;
import org.springframework.security.web.authentication.WebAuthenticationDetailsSource;
import org.springframework.stereotype.Component;
import org.springframework.web.filter.OncePerRequestFilter;

import java.io.IOException;

@Component
@RequiredArgsConstructor
public class JwtFilter extends OncePerRequestFilter {

    private final JwtService jwtService;
    private final UserDetailsService userDetailsService;

    @Override
    protected void doFilterInternal(HttpServletRequest request,
                                    HttpServletResponse response,
                                    FilterChain filterChain) throws ServletException, IOException {
        String authHeader = request.getHeader("Authorization");
        if (authHeader == null || !authHeader.startsWith("Bearer ")) {
            filterChain.doFilter(request, response);
            return;
        }
        String token = authHeader.substring(7);
        if (!jwtService.isTokenValid(token)) {
            filterChain.doFilter(request, response);
            return;
        }
        String username = jwtService.getUsernameFromToken(token);
        if (username != null && SecurityContextHolder.getContext().getAuthentication() == null) {
            UserDetails userDetails = userDetailsService.loadUserByUsername(username);
            UsernamePasswordAuthenticationToken authToken =
                    new UsernamePasswordAuthenticationToken(userDetails, null, userDetails.getAuthorities());
            authToken.setDetails(new WebAuthenticationDetailsSource().buildDetails(request));
            SecurityContextHolder.getContext().setAuthentication(authToken);
        }
        filterChain.doFilter(request, response);
    }
}
'@

Set-Content -Encoding UTF8 -Path "$Src\config\SecurityConfig.java" -Value @'
package com.example.shop.config;

import jakarta.servlet.http.HttpServletResponse;
import lombok.RequiredArgsConstructor;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.http.HttpMethod;
import org.springframework.security.config.annotation.method.configuration.EnableMethodSecurity;
import org.springframework.security.config.annotation.web.builders.HttpSecurity;
import org.springframework.security.config.annotation.web.configuration.EnableWebSecurity;
import org.springframework.security.config.http.SessionCreationPolicy;
import org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.security.web.SecurityFilterChain;
import org.springframework.security.web.authentication.UsernamePasswordAuthenticationFilter;

@Configuration
@EnableWebSecurity
@EnableMethodSecurity
@RequiredArgsConstructor
public class SecurityConfig {

    private final JwtFilter jwtFilter;

    @Bean
    public SecurityFilterChain securityFilterChain(HttpSecurity http) throws Exception {
        http
            .csrf(csrf -> csrf.disable())
            .sessionManagement(session -> session.sessionCreationPolicy(SessionCreationPolicy.STATELESS))
            .authorizeHttpRequests(auth -> auth
                .requestMatchers("/api/auth/**").permitAll()
                .requestMatchers(HttpMethod.GET, "/api/products/**").permitAll()
                .anyRequest().authenticated()
            )
            .addFilterBefore(jwtFilter, UsernamePasswordAuthenticationFilter.class)
            .exceptionHandling(ex -> ex
                .authenticationEntryPoint((request, response, e) ->
                    response.sendError(HttpServletResponse.SC_UNAUTHORIZED, "Unauthorized"))
            );
        return http.build();
    }

    @Bean
    public PasswordEncoder passwordEncoder() {
        return new BCryptPasswordEncoder();
    }
}
'@

Set-Content -Encoding UTF8 -Path "$Src\config\UserDetailsServiceImpl.java" -Value @'
package com.example.shop.config;

import com.example.shop.repository.UserRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.security.core.authority.SimpleGrantedAuthority;
import org.springframework.security.core.userdetails.UserDetails;
import org.springframework.security.core.userdetails.UserDetailsService;
import org.springframework.security.core.userdetails.UsernameNotFoundException;
import org.springframework.stereotype.Service;

import java.util.List;

@Service
@RequiredArgsConstructor
public class UserDetailsServiceImpl implements UserDetailsService {

    private final UserRepository userRepository;

    @Override
    public UserDetails loadUserByUsername(String username) throws UsernameNotFoundException {
        com.example.shop.model.User user = userRepository.findByUsername(username)
                .orElseThrow(() -> new UsernameNotFoundException("User not found: " + username));
        String role = "ROLE_" + user.getRole().name();
        return org.springframework.security.core.userdetails.User.builder()
                .username(user.getUsername())
                .password(user.getPassword())
                .authorities(List.of(new SimpleGrantedAuthority(role)))
                .build();
    }
}
'@

# --------------------------------------------------------------------------- #
# controller
# --------------------------------------------------------------------------- #
Set-Content -Encoding UTF8 -Path "$Src\controller\AuthController.java" -Value @'
package com.example.shop.controller;

import com.example.shop.dto.LoginRequest;
import com.example.shop.dto.RegisterRequest;
import com.example.shop.dto.TokenResponse;
import com.example.shop.service.AuthService;
import lombok.RequiredArgsConstructor;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping("/api/auth")
@RequiredArgsConstructor
public class AuthController {

    private final AuthService authService;

    @PostMapping("/register")
    public ResponseEntity<TokenResponse> register(@RequestBody RegisterRequest request) {
        return ResponseEntity.ok(authService.register(request));
    }

    @PostMapping("/login")
    public ResponseEntity<TokenResponse> login(@RequestBody LoginRequest request) {
        return ResponseEntity.ok(authService.login(request));
    }
}
'@

Set-Content -Encoding UTF8 -Path "$Src\controller\ProductController.java" -Value @'
package com.example.shop.controller;

import com.example.shop.dto.ProductDto;
import com.example.shop.service.ProductService;
import lombok.RequiredArgsConstructor;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.security.access.prepost.PreAuthorize;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/api/products")
@RequiredArgsConstructor
public class ProductController {

    private final ProductService productService;

    @GetMapping
    public ResponseEntity<List<ProductDto>> getAll() {
        return ResponseEntity.ok(productService.getAll());
    }

    @GetMapping("/{id}")
    public ResponseEntity<ProductDto> getById(@PathVariable Long id) {
        return ResponseEntity.ok(productService.getById(id));
    }

    @PostMapping
    @PreAuthorize("hasRole('ADMIN')")
    public ResponseEntity<ProductDto> create(@RequestBody ProductDto dto) {
        return ResponseEntity.status(HttpStatus.CREATED).body(productService.create(dto));
    }

    @PutMapping("/{id}")
    @PreAuthorize("hasRole('ADMIN')")
    public ResponseEntity<ProductDto> update(@PathVariable Long id, @RequestBody ProductDto dto) {
        return ResponseEntity.ok(productService.update(id, dto));
    }

    @DeleteMapping("/{id}")
    @PreAuthorize("hasRole('ADMIN')")
    public ResponseEntity<Void> delete(@PathVariable Long id) {
        productService.delete(id);
        return ResponseEntity.noContent().build();
    }
}
'@

Set-Content -Encoding UTF8 -Path "$Src\controller\OrderController.java" -Value @'
package com.example.shop.controller;

import com.example.shop.dto.CreateOrderRequest;
import com.example.shop.dto.OrderDto;
import com.example.shop.service.OrderService;
import lombok.RequiredArgsConstructor;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.security.core.userdetails.UserDetails;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/api/orders")
@RequiredArgsConstructor
public class OrderController {

    private final OrderService orderService;

    @GetMapping
    public ResponseEntity<List<OrderDto>> getMyOrders(@AuthenticationPrincipal UserDetails userDetails) {
        return ResponseEntity.ok(orderService.getOrdersForUser(userDetails.getUsername()));
    }

    @PostMapping
    public ResponseEntity<OrderDto> createOrder(@AuthenticationPrincipal UserDetails userDetails,
                                                @RequestBody CreateOrderRequest request) {
        return ResponseEntity.status(HttpStatus.CREATED).body(
                orderService.createOrder(userDetails.getUsername(), request));
    }

    @DeleteMapping("/{id}")
    public ResponseEntity<Void> deleteOrder(@PathVariable Long id) {
        orderService.deleteOrder(id);
        return ResponseEntity.noContent().build();
    }
}
'@

Set-Content -Encoding UTF8 -Path "$Src\ShopApplication.java" -Value @'
package com.example.shop;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;

@SpringBootApplication
public class ShopApplication {
    public static void main(String[] args) {
        SpringApplication.run(ShopApplication.class, args);
    }
}
'@

Write-Host ""
Write-Host "[S3] Готово! Файлы созданы в: $Base"
Write-Host ""
Write-Host "  Для офлайн-сборки:"
Write-Host "    cd $Base"
Write-Host "    mvn package -o -Dmaven.repo.local=..\..\libs\maven"
